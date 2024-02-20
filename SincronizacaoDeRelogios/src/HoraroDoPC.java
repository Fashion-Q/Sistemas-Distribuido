import java.time.LocalDateTime;

public class HoraroDoPC {
    public String GetHorarioAtualDoPc()
    {
        LocalDateTime horaAtual = LocalDateTime.now();
        int[][] horarios = {
            {8, 1, 50},
            {10, 10, 10},
            {16, 15, 55}
        };
        int somaHoras = 0;
        int somaMinutos = 0;
        int somaSegundos = 0;
        for (int[] horario : horarios) {
            somaHoras += horario[0];
            somaMinutos += horario[1];
            somaSegundos += horario[2];
        }
        int mediaHoras = somaHoras / horarios.length;
        int mediaMinutos = somaMinutos / horarios.length;
        int mediaSegundos = somaSegundos / horarios.length;
        mediaMinutos += (somaMinutos % horarios.length) / horarios.length;
        mediaSegundos += (somaSegundos % horarios.length) / horarios.length;
        if (mediaSegundos >= 60) {
            mediaSegundos %= 60;
            mediaMinutos++;
        }

        if (mediaMinutos >= 60) {
            mediaMinutos %= 60;
            mediaHoras++;
        } 
        if (mediaHoras >= 24) {
            mediaHoras %= 24;
        }
        // Exibir a média
        System.out.printf("Média: %02d:%02d:%02d%n", mediaHoras, mediaMinutos, mediaSegundos);
        return "Hora atual do sistema: " + horaAtual;
    }
}
