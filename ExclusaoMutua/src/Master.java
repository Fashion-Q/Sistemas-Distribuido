public class Master implements Runnable{
    
    @Override
    public void run()
    {
         sleep(100);
         
    }

    public void sleep(int mil)
    {
        try {
            Thread.sleep(mil);
        } catch (Exception e) {
            // TODO: handle exception
        }
    }
}
