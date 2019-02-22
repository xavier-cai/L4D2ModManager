namespace L4D2ModManager
{
    static class WinformAdapter
    {
        static public void ThreadSleep(uint miiliseconds)
        {
            System.Threading.Thread.Sleep((int)miiliseconds);
        }
    }
}
