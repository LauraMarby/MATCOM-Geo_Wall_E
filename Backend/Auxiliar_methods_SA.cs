using G_Wall_E;
using INTERPRETE_C_to_HULK;
namespace INTERPRETE_C__to_HULK
{
	public partial class Semantic_Analyzer
	{
		private void Sub_Save_Sequence_Value(Node fam)
		{
			//itero por las variables con sus valores y las guardo en el diccionario
			foreach (Node child in fam.Children)
			{
				string name = child.Type;
				dynamic value = child.Value;

				// Si el nombre de la variable coincide con el nombre de una función existente, lanza una excepción
				if (Function_Exist(name)) Input_Error("The variable " + name + " already has a definition as a function in the current context");

				//si la variable ya existe en el diccionario, lanzar excepcion
				if (variables_globales.ContainsKey(name)) Input_Error("The variable " + name + " already has a definition in the current context");

				else variables_globales.Add(name, value);
			}
		}

		//metoo para intentar cconvertir los elementos de una lista de objects a un tipo T
		private void TryConvert<T>(List<object> list1, List<T> list2)
		{
			foreach (object x in list1)
			{
				if (x is not T) Input_Error("Sorry, " + x + " is not a valid type");
				list2.Add((T)x);
			}
		}

		//saber si un object es int
		private bool IsInt(string objectt)
		{
			foreach (char x in objectt) if (x == '.' || !char.IsDigit(x)) return false;
			return true;
		}

		//metodo para saber si un objeto contenido en una lista es valido para una secuencia
		private int IsKind_Seq(object figure)
		{
			if (figure is Point) return 1;
			if (figure is Line) return 2;
			if (figure is Segment) return 3;
			if (figure is Ray) return 4;
			if (figure is Arc) return 5;
			if (figure is Circle) return 6;
			if (figure is string) return 7;
			if (figure is double) return 8;
			if (figure is float) return 9;
			else return -1;
		}

		//saber si un objecto es una secuencia
		private (bool, int) IsSequence<T>(T objectt)
		{
			if (objectt is PointSequence) return (true, 1);
			if (objectt is LineSequence) return (true, 2);
			if (objectt is SegmentSequence) return (true, 3);
			if (objectt is RaySequence) return (true, 4);
			if (objectt is ArcSequence) return (true, 5);
			if (objectt is CircleSequence) return (true, 6);
			if (objectt is StringSequence) return (true, 7);
			if (objectt is IntSequence) return (true, 8);
			if (objectt is FloatSequence) return (true, 9);
			return (false, 0);
		}

		//metodo que evalua la secuencia en el if y determina si representa true o false en el llamado
		private object Evaluate_Sequence_As_Boolean(int a, object sequence, Node node)
		{
			switch (a)
			{
				case 1:
					PointSequence s = (PointSequence)sequence;
					if ((s.values.Count == 1 && s.values[0].Count == 0) || s.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 2:
					LineSequence l = (LineSequence)sequence;
					if ((l.values.Count == 1 && l.values[0].Count == 0) || l.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 3:
					SegmentSequence ss = (SegmentSequence)sequence;
					if ((ss.values.Count == 1 && ss.values[0].Count == 0) || ss.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 4:
					RaySequence r = (RaySequence)sequence;
					if ((r.values.Count == 1 && r.values[0].Count == 0) || r.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 5:
					ArcSequence aa = (ArcSequence)sequence;
					if ((aa.values.Count == 1 && aa.values[0].Count == 0) || aa.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 6:
					CircleSequence c = (CircleSequence)sequence;
					if ((c.values.Count == 1 && c.values[0].Count == 0) || c.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 7:
					StringSequence sss = (StringSequence)sequence;
					if ((sss.values.Count == 1 && sss.values[0].Count == 0) || sss.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				case 8:
					IntSequence i = (IntSequence)sequence;
					if ((i.values.Count == 1 && i.values[0].Count == 0) || i.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
				default:
					FloatSequence f = (FloatSequence)sequence;
					if ((f.values.Count == 1 && f.values[0].Count == 0) || f.is_undefined) return Evaluate(node.Children[2]);
					return Evaluate(node.Children[1]);
			}
		}

		/*private void Evaluate_Sequences_Inside_A_Sequence<T>(ISequence<T> sequence, List<ISequence<T>> list)
		{
			foreach(var x in list)
			{
				sequence = sequence.Concat(x);
			}
		}*/

		private void Draw_Sequence<T>(ISequence<T> s)
		{
			//no admite secuecnias i fiitas o indefinidas
			if (s.is_infinite || s.is_undefined) Input_Error("Infinie or undefined sequences can't be drawed");

			else
			{
				foreach (var c in s.values)
				{
					foreach (var v in c)
					{
						//dibujar secuencias no admiten texto de entrada 
						IDrawable fig = (IDrawable)v;
						Drawables.Add(fig);
					}
				}
			}
		}

		/// <summary>
		/// Metodo para almacenar y asignar las variables que se declaran en el LET
		/// </summary>
		private void Save_Var(Node Children_assigment_list)
		{
			// Crea un nuevo diccionario para almacenar las variables del bloque Let
			Dictionary<string, dynamic> Var_let_in = new Dictionary<string, dynamic>();
			// Añade todas las variables del ámbito actual al nuevo diccionario
			foreach (string key in Scopes[Scopes.Count - 1].Keys)
			{
				Var_let_in.Add(key, Scopes[Scopes.Count - 1][key]);
			}
			// Para cada asignación en la lista de asignaciones, evalúa el valor y añade la variable al nuevo diccionario
			foreach (Node Child in Children_assigment_list.Children)
			{
				string? name = Child.Children[0].Value.ToString();
				dynamic? value = Evaluate(Child.Children[1]);

				// Si el nombre de la variable coincide con el nombre de una función existente, lanza una excepción
				if (Function_Exist(name))
				{
					Input_Error("The variable " + name + " already has a definition as a function in the current context");
				}

				// Si la variable ya existe en el diccionario, actualiza su valor
				if (Var_let_in.ContainsKey(name))
				{
					Var_let_in[name] = value;
				}

				else
				{
					Var_let_in.Add(name, value);
				}
			}
			// Añade el nuevo diccionario de variables al ámbito actual
			Scopes.Add(Var_let_in);
		}

		/// <summary>
		/// Metodo que llama a una funcion ya declarada
		/// </summary>
		private int Call_Function(List<Function_B> f, string name, Node param)
		{
			bool is_found = false;
			// Recorre la lista de funciones declaradas
			for (int i = 0; i < f.Count; i++)
			{
				// Si encuentra una función con el mismo nombre
				if (f[i].Name_function == name)
				{
					is_found = true;
					// Si la función tiene el mismo número de parámetros
					if (f[i].variable_param.Count == param.Children.Count)
					{
						// Añade todas las variables del ámbito anterior al ámbito actual
						foreach (string key in Scopes[Scopes.Count - 2].Keys)
						{
							Scopes[Scopes.Count - 1].Add(key, Scopes[Scopes.Count - 2][key]);
						}

						int count = 0;
						// Para cada parámetro de la función, actualiza su valor en el ámbito actual
						foreach (string key in f[i].variable_param.Keys)
						{
							f[i].variable_param[key] = param.Children[count].Value;
							if (Scopes[Scopes.Count - 1].ContainsKey(key))
							{
								Scopes[Scopes.Count - 1][key] = Evaluate((Node)param.Children[count].Value);
								count++;
							}
							else
							{
								Scopes[Scopes.Count - 1].Add(key, Evaluate((Node)param.Children[count].Value));
								count++;
							}
						}

						return i;
					}
					// Si no coincide el numero de parametros de la funcion con los introducidos a la hora de llamarla
					//lanza un error
					else
					{
						Input_Error("Function " + name + " receives " + f[i].variable_param.Count + " argument(s), but " + param.Children.Count + " were given.");
					}
				}
			}
			// Si no se encuentra la funcion, no se ha declarado, lanza un error
			if (!is_found)
			{
				Input_Error("The function " + name + " has not been declared");
			}

			return -1;
		}


		/// <summary>
		/// Metodo que verifica si la funcion existe declarada
		/// </summary>
		private bool Function_Exist(string? name)
		{
			// Recorre la lista de funciones declaradas
			foreach (Function_B b in functions_declared)
			{
				// Si encuentra una función con el mismo nombre, retorna true
				if (b.Name_function == name)
				{
					return true;
				}
			}
			// Si no encuentra ninguna función con ese nombre, retorna false
			return false;
		}

		/// <summary>
		/// Método que lanza una excepción con un mensaje de error semantico
		/// </summary>
		private void Input_Error(string error)
		{
			throw new Exception("SEMANTIC ERROR: " + error);
		}

		/// <summary>
		/// Metodo que verifica si dos valores son del mismo tipo (del tipo desperado)
		/// </summary>
		private void Type_Expected(object value1, object value2, string type, string op)
		{
			// Si los valores son del tipo esperado, no hace nada
			if (value1 is double && value2 is double && type == "number")
			{
				return;
			}
			else if (value1 is string && value2 is string && type == "string")
			{
				return;
			}
			else if (value1 is bool && value2 is bool && type == "boolean")
			{
				return;
			}
			// Si los valores no son del tipo esperado, lanza una excepción
			else
			{
				Input_Error("Operator \'" + op + "\' cannot be used between \'" + Identify(value1) + "\' and \'" + Identify(value2) + "\'");
			}
		}

		/// <summary>
		/// Metodo que dependiendo del tipo esperado, verifica si el valor es de ese tipo
		/// </summary>
		private void Expected(object value1, string type, string express)
		{
			string v1_type = Identify(value1);

			if (v1_type == type) return;

			switch (type)
			{
				case "string":
					if (value1 is string)
						return;
					break;
				case "number":
					if (value1 is double)
						return;
					break;
				case "bool":
					(bool b, _) = IsSequence(value1);
					if (value1 is bool || value1 is double || b || value1 is string)  //anadido si la expresion booleana es un numero, secuencia o booleana
						return;
					break;
				default:
					Input_Error("The \'" + express + "\' expression must receive type \'" + value1 + "\'");
					break;
			}

			Input_Error("The \'" + express + "\' expression must receive type \'" + type + "\'");

		}

		private string Identify(object value)
		{
			if (value is string) return "string";
			if (value is double) return "number";
			if (value is bool) return "bool";
			return "Unknown";
		}

	}
}