import java.io.FileWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.SocketTimeoutException;
import java.time.LocalDateTime;

public class ExclusaoMutua implements Runnable {
    String pathAtual = System.getProperty("user.dir");

    public ExclusaoMutua(int id) {
        this.id = id;
    }

    private int id;
    int portaMaster = 5998;
    String grupoMasterParaThreads = "239.0.0.2";
    String grupoThreadsParaMaster = "239.0.0.1";
    int portaThreads = 5999;
    boolean acesseiRegiaoCritica = false;
    boolean encerrarPrograma = false;

    @Override
    public void run() {
        if (pathAtual.endsWith("src")) {
            pathAtual = pathAtual.substring(0, pathAtual.length() - 4); // Remova os últimos 4 caracteres ("src\")
            pathAtual+="\\areacritica\\";
        }
        else
        {
            System.err.println("Algo deu errado: " + pathAtual);
            return;
        }
        while (!acesseiRegiaoCritica) {
            esperandoServidorFicarDisponivel();
            if (encerrarPrograma)
            {
                System.err.println("Slave encerrando");
                break;
            }
            enviandoMinhaSolicitacao();
            esperandoRespostaParaVerSeFuiEscolhido();
            if (acesseiRegiaoCritica) {
                acessandoRegiaoCritica();
            }
        }
    }

    public void acessandoRegiaoCritica() {
        try {
            FileWriter file = new FileWriter(pathAtual + "master.txt", true);
            LocalDateTime horaAtual = LocalDateTime.now();
            String horaString = horaAtual.toString();
            file.write("Solicitao feita pelo id: " + id + " no tempo: " +horaString + "\n");
            file.close();
        } catch (Exception e) {
            System.err.println("Excessao");
        }
        try {
            sleep(300);
            String termineiDeAcessarRegiaoCritica = "Over " + id;
            byte[] mensagemByte = termineiDeAcessarRegiaoCritica.getBytes();
            InetAddress addr = InetAddress.getByName(grupoThreadsParaMaster);
            DatagramPacket pkg = new DatagramPacket(mensagemByte, mensagemByte.length, addr, portaMaster);
            DatagramSocket ds = new DatagramSocket();
            ds.send(pkg);
            ds.close();
        } catch (Exception e) {

        }
    }

    public void esperandoRespostaParaVerSeFuiEscolhido() {
        try {
            MulticastSocket mcs = new MulticastSocket(portaThreads);
            InetAddress group = InetAddress.getByName(grupoMasterParaThreads);
            mcs.joinGroup(group);
            try {
                byte rec[] = new byte[256];
                DatagramPacket dataPacket = new DatagramPacket(rec, rec.length);
                mcs.setSoTimeout(10000);
                String dataReceived = "";
                mcs.receive(dataPacket);
                dataReceived = new String(dataPacket.getData(), 0, dataPacket.getLength());
                if (dataReceived.equals(String.valueOf(id))) {
                    acesseiRegiaoCritica = true;
                }
            } catch (SocketTimeoutException e) {

            }
            mcs.close();
        } catch (Exception e) {

        }
    }

    public void enviandoMinhaSolicitacao() {
        try {
            sleep(1000);
            byte[] mensagemByte = String.valueOf(id).getBytes();
            InetAddress addr = InetAddress.getByName(grupoThreadsParaMaster);
            DatagramPacket pkg = new DatagramPacket(mensagemByte, mensagemByte.length, addr, portaMaster);
            DatagramSocket ds = new DatagramSocket();
            ds.send(pkg);
            ds.close();
            System.err.println(id + " Slave ID enviado");
        } catch (Exception e) {

        }
    }

    public void esperandoServidorFicarDisponivel() {

        try {
            MulticastSocket mcs = new MulticastSocket(portaThreads);
            InetAddress group = InetAddress.getByName(grupoMasterParaThreads);
            mcs.joinGroup(group);
            try {
                byte rec[] = new byte[256];
                DatagramPacket dataPacket = new DatagramPacket(rec, rec.length);
                mcs.setSoTimeout(10000);
                String dataReceived = "not";
                while (!dataReceived.contains("Servidor disponivel")) {
                    System.err.println(id + " Slave esperando servidor ficar disponivel");
                    mcs.receive(dataPacket);
                    dataReceived = new String(dataPacket.getData(), 0, dataPacket.getLength());
                    System.err.println("Slave: "+ id + " | " + dataReceived);
                }
            } catch (SocketTimeoutException e) {
                System.err.println("Tempo da slave esperando acabou... "+ id);
                encerrarPrograma = true;
            }
            mcs.close();
        } catch (Exception e) {

        }
    }

    public void sleep(int mil) {
        try {
            Thread.sleep(mil);
        } catch (Exception e) {

        }
    }
}
