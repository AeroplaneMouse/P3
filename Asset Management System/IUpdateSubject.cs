namespace Asset_Management_System
{
    public interface IUpdateSubject
    {
        void Attach(IUpdateObserver observer);
        
        void Detach(IUpdateObserver observer);
        
        void Notify(bool delete);
    }
}