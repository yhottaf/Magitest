using PlayFab;
using System;

/// <summary>
/// 想定外のPlayFabのエラーを例外として扱うためのException
/// </summary>
public class PlayFabErrorException:Exception
{
    public PlayFabErrorException(PlayFabError error):base(error.GenerateErrorReport())
    { }
}
