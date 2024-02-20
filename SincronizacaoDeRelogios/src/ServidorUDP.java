import java.net.*;

public class ServidorUDP implements Runnable {
    int myId;//myId == porta que vai abrir
    int idMaster;//idMater == porta do master que será 6000

    public ServidorUDP (int myId, int idMaster)
    {
        this.myId = myId;
        this.idMaster = idMaster;
    }


    @Override
    public void run() {
        try {
            DatagramSocket serverSocket = new DatagramSocket(myId);
            byte[] receiveData = new byte[1024];
            DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
            
        } catch (SocketException e) {

        }
    }
}
