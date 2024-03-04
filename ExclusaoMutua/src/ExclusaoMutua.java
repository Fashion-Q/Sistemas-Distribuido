public class ExclusaoMutua implements Runnable{
    public ExclusaoMutua(int id)
    {
        this.id = id;
    }
    
    private int id;

    @Override
    public void run()
    {
        System.err.println(id);
    }
}
