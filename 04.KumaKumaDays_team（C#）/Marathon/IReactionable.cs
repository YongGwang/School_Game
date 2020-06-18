/// <summary>
/// Interface for everything actionable with player
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public interface IReactionable
{
    void OnEnter(MarathonPlayerManager actor);
    void OnStay(MarathonPlayerManager actor);
    void OnExit(MarathonPlayerManager actor);
}