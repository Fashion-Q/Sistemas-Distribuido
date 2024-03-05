import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.SocketTimeoutException;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class Master implements Runnable {
    List<String> identificarThreads = new ArrayList<String>();
    int portaMaster = 5998;
    String grupoMasterParaThreads = "239.0.0.2";
    String grupoThreadsParaMaster = "239.0.0.1";
    int portaThreads = 5999;
    boolean temSolicitacoes = true;
    Random random = new Random();

    @Override
    public void run() {
        sleep(100);
        while (temSolicitacoes) {
            solicitacaoDizendoQueServidorEstaDisponivel();
            coletandoSolicitacoesParaAreaCritica();
            if(identificarThreads.size() == 0) // saber se nao tem ninguem mais esperando
            {
                break;
            }
            escolhendoUmaThreadParaAcessarRegiaoCritica();
            esperandoThreadTerminarDeAcessarRegiaoCritica();
        }
    }

    public void solicitacaoDizendoQueServidorEstaDisponivel()
    {
        try {
            sleep(200);
            byte[] mensagemByte = "Servidor disponivel".getBytes();
            InetAddress addr = InetAddress.getByName(grupoMasterParaThreads);
            DatagramPacket pkg = new DatagramPacket(mensagemByte, mensagemByte.length, addr, portaThreads);
            DatagramSocket ds = new DatagramSocket();
            ds.send(pkg);
            ds.close();
        } catch (Exception e) {

        }
    }

    public void esperandoThreadTerminarDeAcessarRegiaoCritica() {
        identificarThreads.clear();
        System.err.println("Esperando a thread acessar a regiao critica...");
        try {
            MulticastSocket mcs = new MulticastSocket(portaMaster);
            InetAddress group = InetAddress.getByName(grupoThreadsParaMaster);
            mcs.joinGroup(group);
            temSolicitacoes = false;
            try {
                byte rec[] = new byte[256];
                DatagramPacket dataPacket = new DatagramPacket(rec, rec.length);
                mcs.setSoTimeout(3000);
                String dataReceived = "not";
                while (!dataReceived.contains("Over")) {
                    mcs.receive(dataPacket);
                    temSolicitacoes = true;
                    dataReceived = new String(dataPacket.getData(), 0, dataPacket.getLength());
                }
                System.err.println("Terminou de acessar a regiao critica: " + dataReceived);
            } catch (SocketTimeoutException e) {

            }
            mcs.close();
        } catch (Exception e) {

        }
    }

    public void escolhendoUmaThreadParaAcessarRegiaoCritica() {
        String threadEscolhida = identificarThreads.get(random.nextInt(identificarThreads.size()));
        System.err.println("Thread escolhida: " + threadEscolhida);
        try {
            sleep(100);
            byte[] mensagemByte = threadEscolhida.getBytes();
            InetAddress addr = InetAddress.getByName(grupoMasterParaThreads);
            DatagramPacket pkg = new DatagramPacket(mensagemByte, mensagemByte.length, addr, portaThreads);
            DatagramSocket ds = new DatagramSocket();
            ds.send(pkg);
            ds.close();
        } catch (Exception e) {

        }
    }

    public void coletandoSolicitacoesParaAreaCritica() {
        identificarThreads.clear();
        System.err.println("Master coletando solicitacoes...");
        try {
            MulticastSocket mcs = new MulticastSocket(portaMaster);
            InetAddress group = InetAddress.getByName(grupoThreadsParaMaster);
            mcs.joinGroup(group);
            temSolicitacoes = false;
            try {
                byte rec[] = new byte[256];
                DatagramPacket dataPacket = new DatagramPacket(rec, rec.length);
                mcs.setSoTimeout(2500);
                while (true) {
                    mcs.receive(dataPacket);
                    temSolicitacoes = true;
                    String dataReceived = new String(dataPacket.getData(), 0, dataPacket.getLength());
                    identificarThreads.add(dataReceived);
                }

            } catch (SocketTimeoutException e) {
                System.err.println("ID de solicitacoes: ");
                if(identificarThreads.size() > 0)
                    for (String idThreads : identificarThreads) {
                        System.err.println(idThreads);
                    }
                else
                {
                    System.err.println("Nao tem mais solicitacoes");
                }
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
