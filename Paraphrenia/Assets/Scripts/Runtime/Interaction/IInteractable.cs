namespace Runtime.Interaction
{
    public interface IInteractable
    {
        
        public bool IsActive { get; set; }
        
        public void InteractorEnter();
        public void InteractorExit();
        public void DoInteract();
    }
}
