
public class App {
    public static void main(String[] args) throws Exception {
        Thread[] udp = new Thread[1];
		for(int i=0;i< udp.length;i++)
		{
			udp[i] = new Thread(new ServidorUDP(6000, i + 6000));
		}
    }
}
