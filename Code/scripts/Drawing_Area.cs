using Godot;
using System;
using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System.Collections.Generic;

public partial class Drawing_Area : Panel
{
	public List<DrawableProperties> figures;

	public bool draw = false;

	Font defaultFont = ThemeDB.FallbackFont;
	int defaultFontSize = ThemeDB.FallbackFontSize;


	//traslacion
	bool vector_declared;
	Vector2 traslation_vector;
	bool found;

	public override void _Draw()
	{
		vector_declared = false;
		Vector2 traslation_vector = new Vector2(0, 0);
		string text;
		found = false;

		//primero las circunferencias
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "circle")
			{
				//traslada si esta fuera de rango, sino no hace nada
				var vector = Operation_T(f.P1);
				found = true;

				DrawArc(vector, (float)f.Radius, 0, 7, 100, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
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
				found = true;
				//traslada si esta fuera de rango, sino no hace nada
				var vector1 = Operation_T(f.P1);
				found = true;
				var vector2 = Operation_T(f.P2);
				var vector3 = Operation_T(f.P3);

				//hallar pendiendtes de las recas
				m1 = ((float)vector2.Y - (float)vector1.Y) / ((float)vector2.X - (float)vector1.X);
				m2 = ((float)vector3.Y - (float)vector1.Y) / ((float)vector3.X - (float)vector1.X);
				//hallar angulos con resepecto al eje x (en radianes)
				//angle_1 = (float)Math.Atan(m1);
				//angle_2 = (float)Math.Atan(m2);
				angle_1 = (float)Math.Atan2((float)vector2.Y - (float)vector1.Y, (float)vector2.X - (float)vector1.X);
				angle_2 = (float)Math.Atan2((float)vector3.Y - (float)vector1.Y, (float)vector3.X - (float)vector1.X);
				DrawArc(vector1, (float)f.Radius, angle_1, angle_2, 60, Paint(f.Color), 1);
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector1, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
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
				//traslada si esta fuera de rango, sino no hace nada
				var vector1 = Operation_T(f.P1);
				found = true;
				var vector2 = Operation_T(f.P2);

				var points = RectIntersection(vector1, vector2, 1152, 648, 0, 0);

				DrawLine(points[0], points[1], Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector1, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
			}
		}

		//segmentos 
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "segment")
			{
				//traslada si esta fuera de rango, sino no hace nada
				var vector1 = Operation_T(f.P1);
				found = true;
				var vector2 = Operation_T(f.P2);

				DrawLine(vector1, vector2, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector1, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
			}
		}

		//rayos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "ray")
			{
				//traslada si esta fuera de rango, sino no hace nada
				var vector1 = Operation_T(f.P1);
				found = true;
				var vector2 = Operation_T(f.P2);

				var point = RayBorder(RectIntersection(vector1, vector2, 1152, 648, 0, 0), vector1, vector2);

				DrawLine(vector1, point, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector1, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
			}
		}

		//puntos
		foreach (DrawableProperties f in figures)
		{
			if (f.Type == "point")
			{
				//traslada si esta fuera de rango, sino no hace nada
				var vector = Operation_T(f);
				found = true;

				DrawCircle(vector, 5, Paint(f.Color));
				text = f.Msg;
				if (text is not null) DrawString(defaultFont, vector, text, HorizontalAlignment.Left, 200, defaultFontSize, Colors.White);
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

	//metodo para verificar si un punto se sale de los limites
	public bool Is_Out_Of_Range(DrawableProperties point)
	{
		return point.X <= 50 || point.X >= 500 || point.Y <= 50 || point.Y >= 600;
	}

	//metodo para hallar el vector coordenada
	public Vector2 Find_Tras_Vector(DrawableProperties point)
	{
		double x = 0;
		double y = 0;

		if (point.X <= 50 || point.X >= 500)
		{
			x = (double)((-1) * point.X + 200);
		}
		if (point.Y <= 50|| point.Y >= 600)
		{
			y = (double)((-1) * point.Y + 300);
		}

		return new Vector2((float)x, (float)y);
	}

	//metodo para trasladar un punto dado un vector de traslacion
	public Vector2 Traslation(DrawableProperties point, Vector2 traslation_vector)
	{
		var x = point.X + traslation_vector.X;
		var y = point.Y + traslation_vector.Y;

		return new Vector2((float)x, (float)y);
	}

	//metodo para encapsular el proceso de traslacion
	public Vector2 Operation_T(DrawableProperties point)
	{
		//si el punto esta fuera de rango
		if (Is_Out_Of_Range(point))
		{
			//si no esta declarado, halla el vector y traslada el punto
			if (!vector_declared && !found)
			{
				traslation_vector = Find_Tras_Vector(point);
				vector_declared = true;
			}
			//si el vector traslacion ya esta declarado, traslada el punto
			return Traslation(point, traslation_vector);
		}
		return new Vector2((float)point.X, (float)point.Y);
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
