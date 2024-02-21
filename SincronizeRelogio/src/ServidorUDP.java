import java.net.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

public class ServidorUDP implements Runnable {
    int myId;//myId == porta que vai abrir
    int idMaster;//idMater == porta do master que será 6000
    int portaMasterMultCast = 5998;
    int portaSlaveMultCast = 5999;
    int novaMediaDeHorarioEmMinutos;
    LocalDateTime myTimeZone;
    List<String> horasEmMinutosDosServidores;

    public ServidorUDP (int portaNormal, int portaMaster, LocalDateTime myTimeZone)
    {
        this.myId = portaNormal;
        this.idMaster = portaMaster;
        this.myTimeZone = myTimeZone;
    }

    @Override
    public void run() {
        if(myId == idMaster)
        {
            masterUDP();
        }
        else
        {
            slaveUDP();
        }
    }
    public void masterUDP()
    {
        System.err.println("Preparando...");
        //Master espera 2 segundos antes de enviar mensagem UDP para que
        //todos os slaves consigam abrir a porta e esperar a mensagem do master
        sleep(2000);
        System.err.println("Começando...");
        //Master envia a primeira mensagem dizendo que quer os horarios dos slides
        enviarMultcastUDP("Manda a hora, slaves!","239.0.0.2",portaSlaveMultCast);
        //Apos o master enviar a mensagem, o master fica esperando 3 segundos ate os slaves
        //enviar suas mensagens, e o master guarda na lista de string
        masterEsperaRespostas();
        //Apos o slive coletar todos os horarios, ele faz a media e guarda na variavel novaMediaDeHorarioEmMinutos
        masterCalculaNovoHorario();
        //Agora o master tem o novo horario, entao ele envia para todos os slaves atraves do multCast
        enviarMultcastUDP(String.valueOf(novaMediaDeHorarioEmMinutos), "239.0.0.2", portaSlaveMultCast);
        //Essa funcao serve tanto para Slave quanto para Master, pois sao a mesma classe
        //Apenas calcula a diferenca entre o horario atual e o novo horario e imprime
        resultadoFinalDeHorario();
    }
    public void masterCalculaNovoHorario()
    {
        int[] horarios = new int[horasEmMinutosDosServidores.size()];
        int somaTotal = 0;
        for(int i=0;i<horasEmMinutosDosServidores.size();i++)
        {
            horarios[i] = Integer.parseInt(horasEmMinutosDosServidores.get(i));
            somaTotal+=horarios[i];
        }
        novaMediaDeHorarioEmMinutos = somaTotal / horasEmMinutosDosServidores.size();
        System.err.println("Novo horário após a média: " + novaMediaDeHorarioEmMinutos);
    }
    public void masterEsperaRespostas()
    {
        horasEmMinutosDosServidores = new ArrayList<String>();
        horasEmMinutosDosServidores.add(pegarHorarioEmMinutos());
        sleep(100);
        System.err.println("Master esperando 3 segundos coletando respostas...");
        try {
            MulticastSocket mcs = new MulticastSocket(portaMasterMultCast);
            InetAddress group = InetAddress.getByName("239.0.0.1");
            mcs.joinGroup(group);
            try {
                byte rec[] = new byte[256];
                DatagramPacket dataPacket = new DatagramPacket(rec, rec.length);
                mcs.setSoTimeout(3000);
                while(true)
                {
                    mcs.receive(dataPacket);
                    String dataReceived = new String(dataPacket.getData(),0,dataPacket.getLength());
                    horasEmMinutosDosServidores.add(dataReceived);
                }
                
            } catch (SocketTimeoutException e) {
                System.err.println("Horários que master recebeu:");
                for (String horaEmMinuto : horasEmMinutosDosServidores) {
                    System.err.println(horaEmMinuto);
                }
            }
            mcs.close();
        } catch (Exception e) {
           
        }
    }
    public void enviarMultcastUDP(String mensagem,String addres, int port)
    {
        try {
            sleep(500);
            byte[] mensagemByte = mensagem.getBytes();
            InetAddress addr = InetAddress.getByName(addres);
            DatagramPacket pkg = new DatagramPacket(mensagemByte, mensagemByte.length,addr,port);
            DatagramSocket ds = new DatagramSocket();
            ds.send(pkg);
            ds.close();
        } catch (Exception e) {
            
        }
    }
    public void slaveUDP()
    {
        //Master esta esperando 2 segundos antes de enviar a mensage, dessa forma
        //o slave tem tempo de chamar a funcao de estar esperando uma mensagem atraves do
        //Multcast
        slaveEsperaMensagem();
        //Quando slave recebe qualquer mensagem, ele sabe que precisa enviar seu horario para o master
        enviarMultcastUDP(pegarHorarioEmMinutos(), "239.0.0.1", portaMasterMultCast);
        //Quando slave envia o horario, ele fica esperando o Master calcular o novo horario
        //na funcao "salveEsperaMensagem()" que retorna a mensagem, e nesse caso, o master
        //vai retornar jah a media final dos horarios
        novaMediaDeHorarioEmMinutos = Integer.parseInt(slaveEsperaMensagem());
        //Agora que slave tem o horario final, ele faz a diferenca entre o horario dele e o novo horario
        //e imprime na tela
        resultadoFinalDeHorario();
    }
    public String slaveEsperaMensagem()
    {
        String data = "None";
        try {
            sleep(100);
            MulticastSocket mcs = new MulticastSocket(portaSlaveMultCast);
            InetAddress grp = InetAddress.getByName("239.0.0.2");
            mcs.joinGroup(grp);
            byte rec[] = new byte[256];
            DatagramPacket pkg = new DatagramPacket(rec, rec.length);
            mcs.receive(pkg);
            data = new String(pkg.getData(), 0, pkg.getLength());
            System.err.println("Slave recebeu: "+ data);
            mcs.close();
        } catch (Exception e) {
            
        }
        return data;
    }
    public String pegarHorarioEmMinutos()
    {
        int hora = myTimeZone.getHour();
        int minuto = myTimeZone.getMinute();
        while(hora > 0)
        {
            minuto = minuto + 60;
            hora--;
        }
        return String.valueOf(minuto);
    }
    public void resultadoFinalDeHorario()
    {
        sleep(200);
        System.err.println(myId + " Meu Horario em Minutos: " + pegarHorarioEmMinutos());
        System.err.println(myId + " Novo Horario em Minutos: " + novaMediaDeHorarioEmMinutos);
        int balance = Integer.parseInt(pegarHorarioEmMinutos());
        balance =  novaMediaDeHorarioEmMinutos - balance;
        sleep(200);
        System.err.println(myId + " Balance: " + balance);
        int hora = 0;
        while(novaMediaDeHorarioEmMinutos >= 60)
        {
            hora++;
            novaMediaDeHorarioEmMinutos-=60;
        }
        sleep(100);
        System.err.println(myId +  " HorarioFinal -> hora: " + hora + " | Minuto: " + novaMediaDeHorarioEmMinutos);
    }
    public void sleep(int millisecond)
    {
        try {
            Thread.sleep(millisecond);
        } catch (Exception e) {
        }
    }
}
