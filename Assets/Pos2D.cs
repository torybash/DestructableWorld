using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct Pos2D {
	public int x;
	public int y;
	public Pos2D(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public static readonly Pos2D up = new Pos2D(0, 1);
	public static readonly Pos2D down = new Pos2D(0, -1);
	public static readonly Pos2D right = new Pos2D(1, 0);
	public static readonly Pos2D left = new Pos2D(-1, 0);
	public static readonly Pos2D zero = new Pos2D(0, 0);
	public static readonly Pos2D one = new Pos2D(1, 1);

	public void Set(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public void Set(Pos2D p) {
		Set(p.x, p.y);
	}
	public void Add(int x, int y) {
		this.x += x;
		this.y += y;
	}
	public void Add(Pos2D p) {
		Add(p.x, p.y);
	}
	public void Subtract(int x, int y) {
		this.x -= x;
		this.y -= y;
	}
	public void Subtract(Pos2D p) {
		Subtract(p.x, p.y);
	}
	public static Pos2D operator +(Pos2D c1, Pos2D c2) {
		return new Pos2D(c1.x + c2.x, c1.y + c2.y);
	}
	public static Pos2D operator -(Pos2D c1, Pos2D c2) {
		return new Pos2D(c1.x - c2.x, c1.y - c2.y);
	}
	public static Pos2D operator -(Pos2D c1) {
		return new Pos2D(-c1.x, -c1.y);
	}
	public static Pos2D operator *(Pos2D c1, Pos2D c2) {
		return new Pos2D(c1.x * c2.x, c1.y * c2.y);
	}
	public static Pos2D operator *(Pos2D c1, int f) {
		return new Pos2D(c1.x * f, c1.y * f);
	}
	public static Pos2D operator /(Pos2D c1, Pos2D c2) {
		return new Pos2D(c1.x / c2.x, c1.y / c2.y);
	}
	public static bool operator ==(Pos2D c1, Pos2D c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}
	public static bool operator !=(Pos2D c1, Pos2D c2) {
		return c1.x != c2.x || c1.y != c2.y;
	}
	public bool onlyOneAxis {
		get { return x == 0 || y == 0; }
	}
	public Pos2D normalized {
		get { return new Pos2D(Mathf.Clamp(x, -1, 1), Mathf.Clamp(y, -1, 1)); }
	}
	public void Normalize() {
		x = Mathf.Clamp(x, -1, 1);
		y = Mathf.Clamp(y, -1, 1);
	}
	public static Pos2D RoundVector2(Vector2 vec){
		return new Pos2D(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
	}
	public override bool Equals(object o) {
		if (o.GetType() == typeof(Pos2D)) {
			Pos2D p = (Pos2D)o;
			return x == p.x && y == p.y;
		}
		return base.Equals(o);
	}
	public override int GetHashCode() {
		return base.GetHashCode();
	}
	public override string ToString() {
		return "(" + x + ", " + y + ")";
	}
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Pos2D))]
public class IngredientDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		SerializedProperty x = property.FindPropertyRelative("x");
		SerializedProperty y = property.FindPropertyRelative("y");

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUIUtility.labelWidth = 14f;

		Rect r = new Rect(position.x, position.y, position.width * 0.5f, position.height);
		x.intValue = EditorGUI.IntField(r, "X", x.intValue);
		r.x += position.width * 0.5f;
		y.intValue = EditorGUI.IntField(r, "Y", y.intValue);

		EditorGUI.EndProperty();
	}
}
#endif