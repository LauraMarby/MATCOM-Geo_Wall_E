using INTERPRETE_C__to_HULK;
using G_Wall_E;
using System;
using System.Collections.Generic;

namespace G_Wall_E
{
    //aqui se guardan todos los metodos necesarios para hallar el intercepto entre dos figuras
    public static class Intersect_Methods
    {
        //PUNTOS
        //con punto
        public static PointSequence Intersect(Point p1, Point p2)
        {
            //si los puntos son iguales retorna cualquiera de los dos puntos
            if (p1.X == p2.X && p1.Y == p2.Y) return new PointSequence(new List<Point>() { p1 });

            //si no lo son retornar secuencia vacia
            else return new PointSequence(new List<Point>());
        }

        //con linea
        public static PointSequence Intersect(Point p, Line l)
        {
            //si intersecta, devuelve el punto
            if (Point_Line(p, l.P1, l.P2)) return new PointSequence(new List<Point>() { p });
            //si no intersecta, devuelve una secuencia vacia
            else return new PointSequence(new List<Point>());
        }

        //con segmento
        public static PointSequence Intersect(Point p, Segment s)
        {
            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //si esta en la recta y en el rango del segmento es intercepto
            if (Point_Line(p, s.P1, s.P2) && Is_Acot(p.X, x_min, x_max) && Is_Acot(p.Y, y_min, y_max))
            {
                return new PointSequence(new List<Point>() { p });
            }
            //no es intercepto
            else return new PointSequence(new List<Point>());
        }

        //con rayo
        public static PointSequence Intersect(Point p, Ray r)
        {
            //si esta dentro de la recta
            if (Point_Line(p, r.P1, r.P2))
            {
                bool acot_x = false; //si esta en le rango de las x
                bool acot_y = false; //si esta en le rango de las y

                //si el rayo se desplaza hacia la izquierda 
                if (r.P1.X > r.P2.X && p.X <= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia la derecha
                else if (r.P1.X < r.P2.X && p.X >= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia abajo
                if (r.P1.Y > r.P2.Y && p.Y <= r.P1.Y) acot_y = true;
                //si el rayo se desplaza hacia arriba
                else if (r.P1.Y < r.P2.Y && p.Y >= r.P1.Y) acot_y = true;

                //si esta acotada, es intercepto
                if (acot_x && acot_y) return new PointSequence(new List<Point>() { p });
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
            //no es intercepto desde un inicio
            else return new PointSequence(new List<Point>());
        }

        //con arco
        public static PointSequence Intersect(Point p, Arc a)
        {
            //hallando el radio de la circunferencia
            var radius = a.Distance.Execute();

            //hallar la acotacion del arco en x
            var x_min = Math.Min(a.P3.X, a.P2.X);
            var x_max = Math.Max(a.P3.X, a.P2.X);
            //hallar la acotacion del arco en y
            var y_min = Math.Min(a.P3.Y, a.P2.Y);
            var y_max = Math.Max(a.P3.Y, a.P2.Y);

            if (Point_Circle(p, a.P1, radius) && Is_Acot(p.X, x_min, x_max) && Is_Acot(p.Y, y_min, y_max))
            {
                return new PointSequence(new List<Point>() { p });
            }

            else return new PointSequence(new List<Point>());
        }

        //con circunferencia
        public static PointSequence Intersect(Point p, Circle c)
        {
            //hallar el radio
            var radius = c.Radius.Execute();
            //es intercepto
            if (Point_Circle(p, c.P1, radius)) return new PointSequence(new List<Point>() { p });
            //no es intercepto
            else return new PointSequence(new List<Point>());
        }

        //LINEAS
        //con linea
        public static PointSequence Intersect(Line l1, Line l2)
        {
            //hallando ecuación de la recta
            var l1_m = ((l1.P2.Y - l1.P1.Y) / (l1.P2.X - l1.P1.X));
            var l1_n = (l1.P1.Y - l1_m * l1.P1.X);
            var l2_m = ((l2.P2.Y - l2.P1.Y) / (l2.P2.X - l2.P1.X));
            var l2_n = (l2.P1.Y - l2_m * l2.P1.X); ;

            //si tienen la misma pendiente son paralelas
            if (l1_m == l2_m)
            {
                //son exactamente iguales
                if (l1_n == l2_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan en un punto
            else
            {
                var xr = (l2_n - l1_n) / (l1_m - l2_m);
                var yr = l1_m * xr + l1_n;
                return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
            }
        }

        //con segmento
        public static PointSequence Intersect(Line l, Segment s)
        {
            //hallando la ecuacion de la recta
            var l_m = ((l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X));
            var l_n = (l.P1.Y - l_m * l.P1.X);
            var s_m = ((s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X));
            var s_n = (s.P1.Y - s_m * s.P1.X);

            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //son paralelas
            if (l_m == s_m)
            {
                //son exactamente iguales
                if (l_n == s_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan en un punto
            else
            {
                var xr = (s_n - l_n) / (l_m - s_m);
                var yr = l_m * xr + l_n;

                //si esta acotada intercepta
                if (Is_Acot(xr, x_min, x_max) && Is_Acot(yr, y_min, y_max))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
                }
                //no esta acotada
                else return new PointSequence(new List<Point>());
            }
        }

        //con rayo
        public static PointSequence Intersect(Line l, Ray r)
        {
            //hallando ecuacion de la recta
            var l_m = ((l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X));
            var l_n = (l.P1.Y - l_m * l.P1.X);
            var r_m = ((r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X));
            var r_n = (r.P1.Y - r_m * r.P1.X);

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                bool acot_x = false; //si esta en le rango de las x
                bool acot_y = false; //si esta en le rango de las y

                //si el rayo se desplaza hacia la izquierda 
                if (r.P1.X > r.P2.X && xr <= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia la derecha
                else if (r.P1.X < r.P2.X && xr >= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia abajo
                if (r.P1.Y > r.P2.Y && yr <= r.P1.Y) acot_y = true;
                //si el rayo se desplaza hacia arriba
                else if (r.P1.Y < r.P2.Y && yr >= r.P1.Y) acot_y = true;

                //si esta acotada, es intercepto
                if (acot_x && acot_y) return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Line l, Arc a)
        {
            //hallando vector de direccion de la recta
            var dx = l.P2.X - l.P1.X;
            var dy = l.P2.Y - l.P1.Y;
            //hallando ecuación paramétrica de la recta
            var A = dx * dx + dy * dy;
            var B = 2 * (dx * (l.P1.X - a.P1.X) + dy * (l.P1.Y - a.P1.Y));
            var C = Math.Pow(l.P1.X - a.P1.X, 2) + Math.Pow(l.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            var det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                var t = -B / (2 * A);
                return Intersect(new Point("_intersection_", "black", l.P1.X + t * dx, l.P1.Y + t * dy), a);
            }
            //intersecta en dos puntos
            else
            {
                var t1 = (-B + Math.Sqrt(det)) / (2 * A);
                var t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", l.P1.X + t1 * dx, l.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", l.P1.X + t2 * dx, l.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0) return P_2;
                if (P_2.Count == 0 && P_1.Count != 0) return P_1;
                if (P_2.Count == 0 && P_1.Count == 0) return P_1;
                return P_1.Concat(P_2);
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Line l, Circle c)
        {
            /*//hallando vector de direccion de la recta
            double dx = l.P2.X - l.P1.X;
            double dy = l.P2.Y - l.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (l.P1.X - c.P1.X) + dy * (l.P1.Y - c.P1.Y));
            double C = Math.Pow((l.P1.X - c.P1.X), 2) + Math.Pow((l.P1.Y - c.P1.Y), 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                return new PointSequence(new List<Point>() { new Point("_intersection_", "black", l.P1.X + t * dx, l.P1.Y + t * dy) });
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                return new PointSequence(new List<Point>() { new Point("_intersection_", "black", l.P1.X + t1 * dx, l.P1.Y + t1 * dy), new Point("_intersection_", "black", l.P1.X + t2 * dx, l.P1.Y + t2 * dy) });
            }*/

            var radius = c.Radius.Execute();

            //Si la distancia del centro a la recta es mayor que el radio, no hay intersección
            if (Distancia_Punto_Recta(c.P1, l.P1, l.P2) > radius)
            {
                return new PointSequence(new List<Point>());
            }

            //Si la distancia del punto a la recta es igual o menor al radio, se intersectan en un solo punto o en dos
            else
            {
                List<Point> Result = new List<Point>();

                //Si no podemos calcular la pendiente por la via trivial, hay que hacerlo de otra forma
                if (l.P2.X - l.P1.X == 0)
                {
                    double X = l.P1.X;
                    double Y = c.P1.Y + Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));
                    Result.Add(new Point("_intersect_", "black", X, Y));
                    Y = c.P1.Y - Math.Sqrt((radius * radius) - ((X - c.P1.X) * (X - c.P1.X)));
                    Result.Add(new Point("_intersect_", "black", X, Y));
                }
                else
                {
                    //Hallando m y n
                    double m = (l.P2.Y - l.P1.Y) / (l.P2.X - l.P1.X);
                    double n = l.P2.Y - (m * l.P2.X);
                    //Parametrizando
                    double A = 1 + (m * m);
                    double B = (2 * m * n) - (2 * c.P1.Y * m) - (2 * c.P1.X);
                    double C = (c.P1.X * c.P1.X) + (c.P1.Y * c.P1.Y) - (radius * radius) - (2 * n * c.P1.Y) + (n * n);
                    double Discriminante = (B * B) - (4 * A * C);
                    //Si el dicriminante es 0, tiene una sola intersección
                    if (Discriminante == 0)
                    {
                        double X = (-B) / 2 * A;
                        double Y = (m * X) + n;
                        Result.Add(new Point("_intersect_", "black", X, Y));
                    }
                    //Si no es 0, tiene 2 intersecciones
                    else
                    {
                        double X = (-B) + Math.Sqrt(Discriminante) / 2 * A;
                        double Y = (m * X) + n;
                        Result.Add(new Point("_intersect_", "black", X, Y));
                        X = (-B) - Math.Sqrt(Discriminante) / 2 * A;
                        Y = (m * X) + n;
                        Result.Add(new Point("_intersect_", "black", X, Y));
                    }
                }
                return new PointSequence(Result);
            }
        }

        //SEGMENTOS
        //con segmento
        public static PointSequence Intersect(Segment s1, Segment s2)
        {
            //hallando la ecuacion de la recta
            var s1_m = ((s1.P2.Y - s1.P1.Y) / (s1.P2.X - s1.P1.X));
            var s1_n = (s1.P1.Y - s1_m * s1.P1.X);
            var s2_m = ((s2.P2.Y - s2.P1.Y) / (s2.P2.X - s2.P1.X));
            var s2_n = (s2.P1.Y - s2_m * s2.P1.X);

            //hallar la acotacion del segmento1 en x
            var x_min_1 = Math.Min(s1.P1.X, s1.P2.X);
            var x_max_1 = Math.Max(s1.P1.X, s1.P2.X);
            //hallar la acotacion del segmento1 en y
            var y_min_1 = Math.Min(s1.P1.Y, s1.P2.Y);
            var y_max_1 = Math.Max(s1.P1.Y, s1.P2.Y);

            //hallar la acotacion del segmento2 en x
            var x_min_2 = Math.Min(s2.P1.X, s2.P2.X);
            var x_max_2 = Math.Max(s2.P1.X, s2.P2.X);
            //hallar la acotacion del segmento2 en y
            var y_min_2 = Math.Min(s2.P1.Y, s2.P2.Y);
            var y_max_2 = Math.Max(s2.P1.Y, s2.P2.Y);

            //son paralelas
            if (s1_m == s2_m)
            {
                //son exactamente iguales
                if (s1_n == s2_n) return new PointSequence(true);
                //no se interceptan
                else return new PointSequence(new List<Point>());
            }
            //se interceptan
            else
            {
                var xr = (s2_n - s1_n) / (s1_m - s2_m);
                var yr = s1_m * xr + s1_n;

                //si esta acotada intercepta
                if (Is_Acot(xr, x_min_1, x_max_1) && Is_Acot(yr, y_min_1, y_max_1) && Is_Acot(xr, x_min_2, x_max_2) && Is_Acot(yr, y_min_2, y_max_2))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
                }
                //no esta acotada
                else return new PointSequence(new List<Point>());
            }
        }

        //con rayo
        public static PointSequence Intersect(Segment s, Ray r)
        {
            //hallando la ecuacion de la recta
            var l_m = ((s.P2.Y - s.P1.Y) / (s.P2.X - s.P1.X));
            var l_n = (s.P1.Y - l_m * s.P1.X);
            var r_m = ((r.P2.Y - r.P1.Y) / (r.P2.X - r.P1.X));
            var r_n = (r.P1.Y - r_m * r.P1.X);

            //hallar la acotacion del segmento en x
            var x_min = Math.Min(s.P1.X, s.P2.X);
            var x_max = Math.Max(s.P1.X, s.P2.X);
            //hallar la acotacion del segmento en y
            var y_min = Math.Min(s.P1.Y, s.P2.Y);
            var y_max = Math.Max(s.P1.Y, s.P2.Y);

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                bool acot_x = false; //si esta en le rango de las x
                bool acot_y = false; //si esta en le rango de las y

                //si el rayo se desplaza hacia la izquierda 
                if (r.P1.X > r.P2.X && xr <= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia la derecha
                else if (r.P1.X < r.P2.X && xr >= r.P1.X) acot_x = true;
                //si el rayo se desplaza hacia abajo
                if (r.P1.Y > r.P2.Y && yr <= r.P1.Y) acot_y = true;
                //si el rayo se desplaza hacia arriba
                else if (r.P1.Y < r.P2.Y && yr >= r.P1.Y) acot_y = true;

                //es intercepto
                if (acot_x && acot_y && Is_Acot(xr, x_min, x_max) && Is_Acot(yr, y_min, y_max))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
                }
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Segment s, Arc a)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - a.P1.X) + dy * (s.P1.Y - a.P1.Y));
            double C = Math.Pow(s.P1.X - a.P1.X, 2) + Math.Pow(s.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s)) return Intersect(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), a);
                else return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s)) return P_2;
                }
                if (P_2.Count == 0 && P_1.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1;
                }
                if (P_2.Count != 0 && P_1.Count != 0)
                {
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1.Concat(P_2);
                    if (!Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_1;
                    if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s)) return P_2;
                }
                return new PointSequence(new List<Point>());
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Segment s, Circle c)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - c.P1.X) + dy * (s.P1.Y - c.P1.Y));
            double C = Math.Pow(s.P1.X - c.P1.X, 2) + Math.Pow(s.P1.Y - c.P1.Y, 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy) });
                }
                return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy) });
                }
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                if (Point_Segment(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Segment(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                return new PointSequence(new List<Point>());
            }
        }

        //RAYOS
        //con rayo
        public static PointSequence Intersect(Ray r1, Ray r2)
        {
            //hallando la ecuacion de la recta
            var l_m = ((r1.P2.Y - r1.P1.Y) / (r1.P2.X - r1.P1.X));
            var l_n = (r1.P1.Y - l_m * r1.P1.X);
            var r_m = ((r2.P2.Y - r2.P1.Y) / (r2.P2.X - r2.P1.X));
            var r_n = (r2.P1.Y - r_m * r2.P1.X);

            //son paralelas
            if (l_m == r_m)
            {
                //son exactamente iguales
                if (l_n == r_n) return new PointSequence(true);
                //no se cortan
                else return new PointSequence(new List<Point>());
            }
            //se cortan
            else
            {
                var xr = (r_n - l_n) / (l_m - r_m);
                var yr = l_m * xr + l_n;

                bool acot_x_1 = false; //si esta en le rango de las x
                bool acot_y_1 = false; //si esta en le rango de las y
                bool acot_x_2 = false; //si esta en le rango de las x
                bool acot_y_2 = false; //si esta en le rango de las y

                //si el rayo se desplaza hacia la izquierda 
                if (r1.P1.X > r1.P2.X && xr <= r1.P1.X) acot_x_1 = true;
                //si el rayo se desplaza hacia la derecha
                else if (r1.P1.X < r1.P2.X && xr >= r1.P1.X) acot_x_1 = true;
                //si el rayo se desplaza hacia abajo
                if (r1.P1.Y > r1.P2.Y && yr <= r1.P1.Y) acot_y_1 = true;
                //si el rayo se desplaza hacia arriba
                else if (r1.P1.Y < r1.P2.Y && yr >= r1.P1.Y) acot_y_1 = true;

                //si el rayo se desplaza hacia la izquierda 
                if (r2.P1.X > r2.P2.X && xr <= r2.P1.X) acot_x_2 = true;
                //si el rayo se desplaza hacia la derecha
                else if (r2.P1.X < r2.P2.X && xr >= r2.P1.X) acot_x_2 = true;
                //si el rayo se desplaza hacia abajo
                if (r2.P1.Y > r2.P2.Y && yr <= r2.P1.Y) acot_y_2 = true;
                //si el rayo se desplaza hacia arriba
                else if (r2.P1.Y < r2.P2.Y && yr >= r2.P1.Y) acot_y_2 = true;

                //es intercepto
                if (acot_x_1 && acot_y_1 && acot_x_2 && acot_y_2)
                {
                    return new PointSequence(new List<Point>() { new Point("_intersect_", "black", xr, yr) });
                }
                //si no lo esta, no lo es
                else return new PointSequence(new List<Point>());
            }
        }

        //con arco
        public static PointSequence Intersect(Ray p, Arc a)
        {
            //hallando vector de direccion de la recta
            double dx = p.P2.X - p.P1.X;
            double dy = p.P2.Y - p.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (p.P1.X - a.P1.X) + dy * (p.P1.Y - a.P1.Y));
            double C = Math.Pow(p.P1.X - a.P1.X, 2) + Math.Pow(p.P1.Y - a.P1.Y, 2) - Math.Pow(a.Distance.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t * dx, p.P1.Y + t * dy), p)) return Intersect(new Point("_intersection_", "black", p.P1.X + t * dx, p.P1.Y + t * dy), a);
                else return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                PointSequence P_1 = Intersect(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), a);
                PointSequence P_2 = Intersect(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), a);
                if (P_1.Count == 0 && P_2.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p)) return P_2;
                }
                if (P_2.Count == 0 && P_1.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1;
                }
                if (P_2.Count != 0 && P_1.Count != 0)
                {
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1.Concat(P_2);
                    if (!Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_1;
                    if (Point_Ray(new Point("_intersection_", "black", p.P1.X + t2 * dx, p.P1.Y + t2 * dy), p) && !Point_Ray(new Point("_intersection_", "black", p.P1.X + t1 * dx, p.P1.Y + t1 * dy), p)) return P_2;
                }
                return new PointSequence(new List<Point>());
            }
        }

        //con circunferencia
        public static PointSequence Intersect(Ray s, Circle c)
        {
            //hallando vector de direccion de la recta
            double dx = s.P2.X - s.P1.X;
            double dy = s.P2.Y - s.P1.Y;
            //hallando ecuación paramétrica de la recta
            double A = dx * dx + dy * dy;
            double B = 2 * (dx * (s.P1.X - c.P1.X) + dy * (s.P1.Y - c.P1.Y));
            double C = Math.Pow(s.P1.X - c.P1.X, 2) + Math.Pow(s.P1.Y - c.P1.Y, 2) - Math.Pow(c.Radius.Value, 2);
            //calculando determinante
            double det = B * B - 4 * A * C;
            //no intersecta en ningún punto
            if (det < 0) return new PointSequence(new List<Point>());
            //intersecta en un solo punto
            else if (det == 0)
            {
                double t = -B / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t * dx, s.P1.Y + t * dy) });
                }
                return new PointSequence(new List<Point>());
            }
            //intersecta en dos puntos
            else
            {
                double t1 = (-B + Math.Sqrt(det)) / (2 * A);
                double t2 = (-B - Math.Sqrt(det)) / (2 * A);
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s) && !Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy) });
                }
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && !Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                if (Point_Ray(new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy), s) && Point_Ray(new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), s))
                {
                    return new PointSequence(new List<Point>() { new Point("_intersection_", "black", s.P1.X + t1 * dx, s.P1.Y + t1 * dy), new Point("_intersection_", "black", s.P1.X + t2 * dx, s.P1.Y + t2 * dy) });
                }
                return new PointSequence(new List<Point>());
            }
        }

        //ARCOS
        //con arco
        public static PointSequence Intersect(Arc c1, Arc c2)
        {
            // Verificar si los arcos son el mismo
            if (c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y && c1.P2.X == c2.P2.X && c1.P2.Y == c2.P2.Y && c1.P3.X == c2.P3.X && c1.P3.Y == c2.P3.Y) return new PointSequence(true);
            double d = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));
            if (d > c1.Distance.Value + c2.Distance.Value || d < Math.Abs(c1.Distance.Value - c2.Distance.Value)) return new PointSequence(new List<Point>());
            double a = Math.Pow(c1.Distance.Value, 2) - Math.Pow(c2.Distance.Value, 2) + d * d / 2 * d;
            double h = Math.Sqrt(Math.Pow(c1.Distance.Value, 2)) - Math.Pow(a, 2);
            double x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / d;
            double y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / d;
            double x1 = x + h * (c2.P1.Y - c1.P1.Y) / d;
            double y1 = y - h * (c2.P1.X - c1.P1.X) / d;
            double x2 = x - h * (c2.P1.Y - c1.P1.Y) / d;
            double y2 = y + h * (c2.P1.X - c1.P1.X) / d;
            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);
            if (x1 == x2 && y1 == y2 && Point_Arc(P1, c1) && Point_Arc(P1, c2)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P1, c1) && Point_Arc(P1, c2) && Point_Arc(P2, c1) && Point_Arc(P2, c2)) return new PointSequence(new List<Point>() { P1, P2 });
            if (Point_Arc(P1, c1) && Point_Arc(P1, c2)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P2, c1) && Point_Arc(P2, c2)) return new PointSequence(new List<Point>() { P2 });
            return new PointSequence(new List<Point>());
        }


        //con circunferencia
        public static PointSequence Intersect(Arc p, Circle c)
        {
            // Verificar si los arcos son el mismo
            if (p.P1.X == c.P1.X && p.P1.Y == p.P1.Y && p.Distance.Value == c.Radius.Value) return new PointSequence(true);
            double d = Math.Sqrt(Math.Pow(c.P1.X - p.P1.X, 2) + Math.Pow(c.P1.Y - p.P1.Y, 2));
            if (d > p.Distance.Value + c.Radius.Value || d < Math.Abs(p.Distance.Value - c.Radius.Value)) return new PointSequence(new List<Point>());
            double a = Math.Pow(p.Distance.Value, 2) - Math.Pow(c.Radius.Value, 2) + d * d / 2 * d;
            double h = Math.Sqrt(Math.Pow(p.Distance.Value, 2)) - Math.Pow(a, 2);
            double x = p.P1.X + a * (c.P1.X - p.P1.X) / d;
            double y = p.P1.Y + a * (c.P1.Y - p.P1.Y) / d;
            double x1 = x + h * (c.P1.Y - p.P1.Y) / d;
            double y1 = y - h * (c.P1.X - p.P1.X) / d;
            double x2 = x - h * (c.P1.Y - p.P1.Y) / d;
            double y2 = y + h * (c.P1.X - p.P1.X) / d;
            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);
            if (x1 == x2 && y1 == y2 && Point_Arc(P1, p)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P1, p) && Point_Arc(P2, p)) return new PointSequence(new List<Point>() { P1, P2 });
            if (Point_Arc(P1, p)) return new PointSequence(new List<Point>() { P1 });
            if (Point_Arc(P2, p)) return new PointSequence(new List<Point>() { P2 });
            return new PointSequence(new List<Point>());
        }

        //CIRCUNFERENCIAS
        //con circunferencia
        public static PointSequence Intersect(Circle c1, Circle c2)
        {
            /*var radiusc1 = c1.Radius.Execute();
            var radiusc2 = c2.Radius.Execute();

            if (c1.P1.X == c2.P1.X && c1.P1.Y == c2.P1.Y && radiusc1 == radiusc2) return new PointSequence(true);

            double d = Math.Sqrt(Math.Pow(c2.P1.X - c1.P1.X, 2) + Math.Pow(c2.P1.Y - c1.P1.Y, 2));

            if (d > radiusc1 + radiusc2 || d < Math.Abs(radiusc1 - radiusc2)) return new PointSequence(new List<Point>());

            double a = Math.Pow(radiusc1, 2) - Math.Pow(radiusc2, 2) + (d * d) / 2 * d;
            double h = Math.Sqrt(Math.Pow(c1.Radius.Value, 2)) - Math.Pow(a, 2);
            double x = c1.P1.X + a * (c2.P1.X - c1.P1.X) / d;
            double y = c1.P1.Y + a * (c2.P1.Y - c1.P1.Y) / d;
            double x1 = x + h * (c2.P1.Y - c1.P1.Y) / d;
            double y1 = y - h * (c2.P1.X - c1.P1.X) / d;
            double x2 = x - h * (c2.P1.Y - c1.P1.Y) / d;
            double y2 = y + h * (c2.P1.X - c1.P1.X) / d;

            Point P1 = new Point("i", "black", x1, y1);
            Point P2 = new Point("i", "black", x2, y2);

            if (x1 == x2 && y1 == y2) return new PointSequence(new List<Point>() { P1 });

            return new PointSequence(new List<Point>() { P1, P2 });*/

            var radius_1 = c1.Radius.Execute();
            var radius_2 = c2.Radius.Execute();

            var a = -2 * (c2.P1.Y - c1.P1.Y);
            var b = -2 * (c1.P1.X + c2.P1.X - 2 * c2.P1.X * (c2.P1.Y - c1.P1.Y));
            var c = Math.Pow(c1.P1.X, 2) + Math.Pow(c2.P1.X, 2) + Math.Pow(radius_1, 2) + Math.Pow(radius_2, 2) + (c2.P1.Y - c1.P1.Y) * (c2.P1.Y - c1.P1.Y + 2 * Math.Abs(radius_2) - 2 * Math.Pow(Math.Abs(c2.P1.X), 2));

            var discriminante = b * b - 4 * a * c;

            if (discriminante < 0) return new PointSequence(new List<Point>());

            else if (discriminante == 0)
            {
                var x = -b / (2 * a);
                var y = Math.Sqrt(Math.Pow(radius_2, 2) - Math.Pow(x - c2.P1.X, 2)) + c2.P1.Y;

                return new PointSequence(new List<Point>() { new Point("_intersect_", "black", x, y) });
            }

            else
            {
                var x_1 = (-b + discriminante) / (2 * a);
                var y_1 = Math.Sqrt(Math.Pow(radius_2, 2) - Math.Pow(x_1 - c2.P1.X, 2)) + c2.P1.Y;

                var x_2 = (-b - discriminante) / (2 * a);
                var y_2 = Math.Sqrt(Math.Pow(radius_2, 2) - Math.Pow(x_2 - c2.P1.X, 2)) + c2.P1.Y;

                return new PointSequence(new List<Point>() { new Point("_intersect_", "black", x_1, y_1), new Point("_intersect_", "black", x_2, y_2) });
            }
        }


        //métodos auxiliares
        public static bool Point_Line(Point p, Point P1, Point P2) //
        {
            //hallando ecuación de la recta
            var m = (P2.Y - P1.Y) / (P2.X - P1.X);
            var n = (P1.Y - m * P1.X);

            //si la igualdad coincide, devuelve p, sino retorna vacío
            if (p.Y == m * p.X + n) return true;
            return false;
        }
        static bool Point_Segment(Point p, Segment s)
        {
            double Producto_cruz = (s.P2.Y - s.P1.Y) * (p.X - s.P1.X) - (s.P2.X - s.P1.X) * (p.Y - s.P1.Y);
            if (Math.Abs(Producto_cruz) > 1e-10) return false;

            double Producto_escalar = (p.X - s.P1.X) * (s.P2.X - s.P1.X) + (p.Y - s.P1.Y) * (s.P2.Y - s.P1.Y);
            if (Producto_escalar < 0)
            {
                return false;
            }

            //calculando la longitud al cuadrado
            double lc = (s.P2.X - s.P1.X) * (s.P2.X - s.P1.X) + (s.P2.Y - s.P1.Y) * (s.P2.Y - s.P1.Y);
            if (Producto_escalar > lc)
            {
                return false;
            }

            return true;
        }
        static bool Point_Ray(Point p, Ray r)
        {
            double Vx = p.X - r.P1.X;
            double Vy = p.Y - r.P1.Y;
            double Rx = r.P2.X - r.P1.X;
            double Ry = r.P2.Y - r.P1.Y;
            double dotProduct = Rx * Vx + Ry * Vy;
            double R_Magnitude = Math.Sqrt(Rx * Rx + Ry * Ry);
            double V_Magnitude = Math.Sqrt(Vx * Vx + Vy * Vy);
            double angle = Math.Acos(dotProduct / (R_Magnitude * V_Magnitude));
            return Math.Abs(angle) < 0.0001;
        }
        static bool Point_Arc(Point p, Arc a)
        {
            // Calcular el ángulo de la línea que conecta el centro del arco con el punto
            double dx = p.X - a.P1.X;
            double dy = p.Y - a.P1.Y;
            double angle = Math.Atan2(dy, dx);

            // Calcular los ángulos de inicio y fin del arco
            double angle1 = Math.Atan2(a.P2.Y - a.P1.Y, a.P2.X - a.P1.X);
            double angle2 = Math.Atan2(a.P3.Y - a.P1.Y, a.P3.X - a.P1.X);

            // Verificar si el punto está dentro del arco
            if (angle >= Math.Min(angle1, angle2) && angle <= Math.Max(angle1, angle2))
            {
                // Calcular la distancia del punto al centro del arco
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // Si la distancia es igual al radio del arco, hay una intersección
                if (distance == a.Distance.Value)
                {
                    return true;
                }
            }
            // Si llegamos aquí, no hay intersección
            return false;
        }

        public static bool Point_Circle(Point p, Point P1, double Radius)//
        {
            //sustituyendo el punto en la ecuacion de la circunferencia
            if (Math.Pow(p.X - P1.X, 2) + Math.Pow(p.Y - P1.Y, 2) == Math.Pow(Radius, 2)) return true;
            //no es intercepto
            else return false;
        }

        public static bool Is_Acot(double a, double min, double max)//
        {
            return a >= min && a <= max;
        }


        public static double Distancia_Punto_Recta(Point punto, Point recta_p1, Point recta_p2)
        {
            double distance;
            if (recta_p1.X == recta_p2.X)
            {
                distance = recta_p1.X - punto.X;
            }
            else if (recta_p1.Y == recta_p2.Y)
            {
                distance = recta_p1.Y - punto.Y;
            }
            else
            {
                //Calculando ecuación cartesiana
                double m = (recta_p2.Y - recta_p1.Y) / (recta_p2.X - recta_p1.X);
                double n = recta_p2.Y - (m * recta_p2.X);
                //Declarando parámetros
                double A = m;
                double B = -1;
                double C = n;
                //Calcuando distancia
                distance = ((A * punto.X) + (B * punto.Y) + C) / Math.Sqrt((A * A) + (B * B));
            }
            //Devolver el módulo de la distancia
            if (distance < 0) return -distance;
            return distance;
        }
    }

}