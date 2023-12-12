using Godot;
using System;
using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System.Collections.Generic;

public partial class Drawing_Area : Panel
{
	public List<DrawableProperties> figures;

	public bool draw = false;

	Font font = (Font)GD.Load("res://images/font.TTF");

	public override void _Draw()
	{
		string text;

		//primero las circunferencias
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "circle")
			{
				DrawCircle(new Vector2((float)f.P1.X, (float)f.P1.Y), (float)f.Radius, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 500, 20, Colors.White);
			}
		}

		float m1;
		float m2;
		float angle_1;
		float angle_2;
		//arcos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "arc")
			{
				//hallar pendiendtes de las recas
				m1 = ((float)f.P2.Y - (float)f.P1.Y) / ((float)f.P1.X - (float)f.P1.X);
				m2 = ((float)f.P3.Y - (float)f.P1.Y) / ((float)f.P3.X - (float)f.P1.X);
				//hallar angulos con resepecto al eje x
				angle_1 = (float)Math.Pow(Math.Tan(m1), -1);
				angle_2 = (float)Math.Pow(Math.Tan(m2), -1);

				if (m1 < 0) angle_1 += (float)Math.PI;
				if (m2 < 0) angle_2 += (float)Math.PI;

				DrawArc(new Vector2((float)f.P1.X, (float)f.P1.Y), (float)f.Radius, angle_1, angle_2, 2, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		double x3;
		double y3;
		double x4;
		double y4;

		//lineas 
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "line")
			{
				var points = RectIntersection(new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)f.P2.X, (float)f.P2.Y), 1152, 648, 0, 0);

				DrawLine(points[0], points[1], Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//segmentos 
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "segment")
			{
				DrawLine(new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)f.P2.X, (float)f.P2.Y), Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//rayos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "ray")
			{
				var point = RayBorder(RectIntersection(new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)f.P2.X, (float)f.P2.Y), 1152, 648, 0, 0), new Vector2((float)f.P1.X, (float)f.P1.Y), new Vector2((float)f.P2.X, (float)f.P2.Y));

				DrawLine(new Vector2((float)f.P1.X, (float)f.P1.Y), point, Colors.Green);
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

		//puntos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "point")
			{
				DrawCircle(new Vector2((float)f.X, (float)f.Y), 5, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(font, new Vector2((float)f.P1.X, (float)f.P1.Y), text, HorizontalAlignment.Left, 200, 200, Colors.White);
			}
		}

	}

	//metodos para hallar limites
	int Limit_X(int m, int n, int x)
	{
		return m * x + n;
	}
	int Limit_Y(int m, int n, int y)
	{
		return (y - n) / m;
	}

	private List<Vector2> RectIntersection(Vector2 p1, Vector2 p2, int MaxX, int MaxY, int MinX, int MinY)
	{
		List<Vector2> Intersection = new List<Vector2>();

		//Calculando pendiente y desplazamiento de la recta
		if (p2.X - p1.X == 0)
		{
			Intersection.Add(new Vector2(p2.X, MinY));
			Intersection.Add(new Vector2(p2.X, MaxY));
		}
		else if (p2.Y - p1.Y == 0)
		{
			Intersection.Add(new Vector2(MinX, p2.Y));
			Intersection.Add(new Vector2(MaxX, p2.Y));
		}
		else
		{
			float m = (p2.Y - p1.Y) / (p2.X - p1.X);
			float n = p2.Y - p2.X * m;
			for (int i = 0; i < 4; i++)
			{
				float X;
				float Y;
				if (i == 0)
				{
					X = (MinY - n) / m;
					Y = (m * X) + n;
				}
				else if (i == 1)
				{
					X = (MaxY - n) / m;
					Y = (m * X) + n;
				}
				else if (i == 2)
				{
					Y = (m * MinX) + n;
					X = (Y - n) / m;
				}
				else
				{
					Y = (m * MaxX) + n;
					X = (Y - n) / m;
				}
				//Si no son válidos, sigue
				if (X > MaxX || X < MinX || Y > MaxY || Y < MinY)
				{
					continue;
				}
				//Si son válidos, entonces agregalos a la lista como intersección
				else
				{
					Intersection.Add(new Vector2(X, Y));
				}
			}
		}
		return Intersection;
	}

	private Vector2 RayBorder(List<Vector2> list, Vector2 p1, Vector2 p2)
	{
		double db1 = Math.Sqrt(Math.Pow(p2.X - list[0].X, 2) + Math.Pow(p2.Y - list[0].Y, 2));
		double db2 = Math.Sqrt(Math.Pow(p2.X - list[1].X, 2) + Math.Pow(p2.Y - list[1].Y, 2));
		if (db1 < db2) return list[0];
		return list[1];
	}

	//metodo que recibe la orden de dibujar
	public void Changed(List<DrawableProperties> drawables)
	{
		figures = drawables;
		draw = true;
	}

	//metodo para asignar colores cuando vengan del input del interprete
	public Color Paint(string color)
	{
		switch (color)
		{
			case "blue":
				return Colors.Blue;

			case "red":
				return Colors.Red;

			case "yellow":
				return Colors.Yellow;

			case "green":
				return Colors.Green;

			case "cyan":
				return Colors.Cyan;

			case "magenta":
				return Colors.Magenta;

			case "gray":
				return Colors.Gray;

			case "black":
				return Colors.Black;
		}
		return Colors.White;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GetNode<Button>("/root/Scene/Fondo/Interact_Area/Confirm_Button").Connect("pressed", Callable.From(Changed));
		_Draw();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (draw)
		{
			QueueRedraw();
			draw = false;
		}
	}
}
