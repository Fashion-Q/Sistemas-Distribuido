public class App {
    public static void main(String[] args) throws Exception {
        Thread[]exclusaoMutua = new Thread[4]; // numero de threads criadas

        for(int i=0;i<exclusaoMutua.length;i++)
        {
            exclusaoMutua[i] = new Thread(new ExclusaoMutua(i));
            exclusaoMutua[i].start();
        }
        Thread master = new Thread(new Master());
        master.start();
    }
}
