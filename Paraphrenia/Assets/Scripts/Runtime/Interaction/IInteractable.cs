namespace Runtime.Interaction
{
    public interface IInteractable
    {
        public void InteractorEnter();
        public void InteractorExit();
        public void DoInteract();
    }
}
