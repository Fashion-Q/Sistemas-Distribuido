import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.net.*;
import java.time.LocalDateTime;

public class ServidorUDP implements Runnable {
    int myId;//myId == porta que vai abrir
    int idMaster;//idMater == porta do master que será 6000
    LocalDateTime myTimeZone;
    int tamanhoServidores;

    public ServidorUDP (int myId, int idMaster, LocalDateTime myTimeZone, int tamanhoServidores)
    {
        this.myId = myId;
        this.idMaster = idMaster;
        this.myTimeZone = myTimeZone;
        this.tamanhoServidores = tamanhoServidores;
    }
    BufferedReader inFromUser;
    BufferedReader inFromServer;
    DataOutputStream outToServer;

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
        byte[] sendData = new byte[1024];
		byte[] receiveData = new byte[1024];
        try {
            //Master cria servidor local
            DatagramSocket clientSocket = new DatagramSocket();
            InetAddress ipAddress = InetAddress.getByName("127.0.0.1");
            sendData = "Mande o horário atual, escravos!".getBytes();
            //Envia pros escravos pedindo a hora do 6001 até tamanhoServidores
            for(int i=1;i<tamanhoServidores;i++)
            {
                DatagramPacket sendPacket = new DatagramPacket(sendData,
					sendData.length, ipAddress, 6000 + i);
                    clientSocket.send(sendPacket);
            }
            //servidorMaster já inclui seu horário no começo da lista de horário (recebe o pacote em minutos)
            String[] listaString = new String[tamanhoServidores];
            int transformarHoraEmMinuto = (myTimeZone.getHour() * 60) + myTimeZone.getMinute();
            listaString[0] = String.valueOf(transformarHoraEmMinuto);

            //master coleta de todos os servidores a hora em minuto e guarda numa string
            for (int i = 1; i < tamanhoServidores; i++) {
                DatagramPacket receivePacket = new DatagramPacket(receiveData,
            	receiveData.length);
                clientSocket.receive(receivePacket);
                listaString[i] = new String(receivePacket.getData(),0,receivePacket.getLength());
            }
            //Master faz a média geral
            double transformarEmMinutos = 0;
            for(int i=0;i<tamanhoServidores;i++)
                transformarEmMinutos = transformarEmMinutos + Double.parseDouble(listaString[i]);
            transformarEmMinutos = transformarEmMinutos / tamanhoServidores;
            //TODO ta perdendo os segundos, fazer com que nao perca depois
            int removerParteFracionaria = (int) transformarEmMinutos;
            sendData = String.valueOf(removerParteFracionaria).getBytes();
            //Mater envia pra todo mundo o novo horário em minutos e eles vao calcular 
            //quanto falta, inclusive o próprio Master calcula o dele
            for(int i=1;i<tamanhoServidores;i++)
            {
                DatagramPacket sendPacket = new DatagramPacket(sendData,
					sendData.length, ipAddress, 6000 + i);
                    clientSocket.send(sendPacket);
            }

            clientSocket.close();
        } catch (SocketException e) {

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    public void slaveUDP()
    {

    }
}
