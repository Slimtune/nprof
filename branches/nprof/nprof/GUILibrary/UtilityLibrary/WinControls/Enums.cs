namespace UtilityLibrary.WinControls
{
	#region DrawState
	public enum DrawState
	{
		Normal,
		Hot,
		Pressed,
		Disable
	}
	#endregion

	#region ScrollBarEvent
	public enum ScrollBarEvent
	{
		LineUp,
		LineDown,
		PageUp,
		PageDown,
		ThumbUp,
		ThumbDown
	}

	#endregion

	#region ScrollBarHit
	public enum ScrollBarHit
	{
		UpArrow,
		DownArrow,
		PageUp,
		PageDown,
		Thumb,
		LeftArrow,
		RightArrow,
		PageLeft,
		PageRight,
		None
	}
	#endregion

	#region Orientation
	public enum Orientation
	{
		Vertical,
		Horizontal
	}
	#endregion

}