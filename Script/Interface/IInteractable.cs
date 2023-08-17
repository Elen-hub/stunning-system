public interface IInteractable : IInfoViewer
{
    public eInteractState InteractState { get; set; }
    public ulong WorldID { get; }
    public void NotifyRegistProximate();
    public void NotifyRemoveProximate();
    public bool IsPossibleInteract(IActor caster);
    public void Interact(IActor caster);
    public void InteractExit();
}
