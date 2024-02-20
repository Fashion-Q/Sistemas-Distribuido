import java.time.LocalDateTime;

public class App {
    public static void main(String[] args) throws Exception {
        Thread[] udp = new Thread[3];
        //primeiro hora e depois minuto, hora = 0 - 23
        LocalDateTime[] timeZone = getDateTimes();
		for(int i=0;i< udp.length;i++)
		{
			udp[i] = new Thread(new ServidorUDP(6000, i + 6000,timeZone[i],udp.length));
		}
    }
    
    public static LocalDateTime[] getDateTimes()
    {
        LocalDateTime[] timeZone = new LocalDateTime[3];
        timeZone[0] = LocalDateTime.of(
            LocalDateTime.now().getYear(),
            LocalDateTime.now().getMonth(),
            LocalDateTime.now().getDayOfMonth(),
            3, // Horas desejadas
            0, // Minutos desejados
            0 // Segundos desejados
        );
        timeZone[1] = LocalDateTime.of(
            LocalDateTime.now().getYear(),
            LocalDateTime.now().getMonth(),
            LocalDateTime.now().getDayOfMonth(),
            2,
            50,
            0
        );
        timeZone[2] = LocalDateTime.of(
            LocalDateTime.now().getYear(),
            LocalDateTime.now().getMonth(),
            LocalDateTime.now().getDayOfMonth(),
            3,
            25,
            0
        );
        return timeZone;
    }
    
}