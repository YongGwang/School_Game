/// <summary>
/// Interface for everything action with player
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public interface IReactionable
{
    void OnEnter(MarathonPlayerManager acter);
    void OnStay(MarathonPlayerManager acter);
    void OnExit(MarathonPlayerManager acter);
}