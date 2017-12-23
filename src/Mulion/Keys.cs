using System;

namespace Mulion{
	public enum Key{
		Escape,
		Space,
		Delete,
		Enter,
		Backspace,
		Home,
		End,
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,
		Left,
		Right,
		Up,
		Down
	}

	[Flags]
	public enum Modifiers{
		Shift = 1,
		Ctrl,
		Alt
	}
}