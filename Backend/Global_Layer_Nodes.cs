using G_Wall_E;
namespace INTERPRETE_C__to_HULK
{
	public partial class Parser
	{
		/// <summary>
		/// Este método se encarga de procesar la importacion de codigo
		/// </summary>
		public Node Import_Code()
		{
			position++;
			if (TS[position].Type == TokenType.STRING)
			{
				string dir = TS[position++].Value.ToString();

				Import imported = new Import(dir);
				string code = imported.Code();

				Lexer T = new Lexer(code);
				List<Token> new_TS = T.Tokens_sequency;

				Parser P = new Parser(new_TS);
				Node N = P.Parse();

				return new Node { Type = "import", Children = new List<Node> { N } };
			}
			Input_Error("Se espera un string con la direccion del archivo a importar");
			return null;
		}

		/// <summary>
		/// Este método se encarga de procesar las asignaciones de variables del lenguaje (LET-IN)
		/// </summary>
		public Node Assigment()
		{
			position++;
			Node assigments = new Node { Type = "assigment_list" };
			bool d_comma = false;

			do
			{
				is_let = true;
				if (d_comma)
				{
					position++;
				}
				d_comma = true;

				Expect(TokenType.VARIABLE, "nombre_de_variable");
				Node name = new Node { Type = "name", Value = TS[position - 1].Value };
				Expect(TokenType.EQUAL, "=");
				Node value = Layer_6();
				Exceptions_Missing(value, "let-in");

				Node var = new Node { Type = "assigment", Children = new List<Node> { name, value } };
				assigments.Children.Add(var);

			} while (TS[position].Type == TokenType.D_COMMA);
			is_let = false;

			Expect(TokenType.IN, "in");
			Node operations = Global_Layer();
			Exceptions_Missing(operations, "let-in");

			Node variable = new Node { Type = "Let", Children = new List<Node> { assigments, operations } };
			return variable;
		}

		/// <summary>
		/// Este método se encarga de procesar los objetos a dibujar (DRAW)
		/// </summary>
		public Node Drawable()
		{
			position++;
			Node expression = null; //nuevo de camila
			Node str = new Node { Type = "empty" };
			Node save_variable = new Node { Type = "save" }; //nuevo de camila

			if (IsGeometricToken()) //nuevo de camila
			{
				NotSaveVariable = true;
				save_variable = new Node { Type = "not_save" };
				expression = Layer_0();
			}
			else //nuevo de camila
			{
				expression = Global_Layer();
			}

			if (TS[position].Type == TokenType.STRING)
			{
				str = Factor();
			}

			return new Node { Type = "draw", Children = new List<Node> { expression, str, save_variable } }; //nuevo de camila
		}

		/// <summary>
		/// Este método se encarga de procesar las medidas entre 2 puntos (MEASURE)
		/// </summary>
		public Node Measure()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node p1 = Factor();
			Expect(TokenType.COMMA, ",");
			Node p2 = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "measure", Children = new List<Node> { p1, p2 } };
		}

		//Este metodo se encarga de hallar el intercepto entre dos figuras
		public Node Intersect()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node f1 = Factor();
			Expect(TokenType.COMMA, ",");
			Node f2 = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "intersect", Children = new List<Node> { f1, f2 } };
		}

		//este metodo se encarga de hallar una secuencia finita de puntos aleatorios en una figura
		public Node Points()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node f = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "points", Children = new List<Node>() { f } };
		}

		//Este metodo se necarga de hallar la cantidad de elementos que tiene una sequencia
		public Node Count()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			is_param = true;
			Node sec = Factor();
			is_param = false;
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "count", Children = new List<Node> { sec } };
		}

		//este metodo se encarga de hallar la secuencia de flotantes
		public Node Randoms()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "randoms", Children = new List<Node>() };
		}

		//este metodo se encarga de halalr la secuencia de puntos en el plano
		public Node Samples()
		{
			position++;

			Expect(TokenType.L_PHARENTESYS, "(");
			Expect(TokenType.R_PHARENTESYS, ")");

			return new Node { Type = "samples", Children = new List<Node>() };
		}
		/// <summary>
		/// Este método se encarga de procesar las estructuras condicionales del lenguaje (IF-ELSE)
		/// </summary>
		public Node Conditional()
		{
			position++;
			Node condition = Layer_6();
			Expect(TokenType.THEN, "then");
			Node operations_if = Global_Layer();
			Expect(TokenType.ELSE, "else");
			Node operations_else = Global_Layer();
			Node conditional_if_else = new Node { Type = "Conditional", Children = new List<Node> { condition, operations_if, operations_else } };
			return conditional_if_else;
		}

		/// <summary>
		/// Este método se encarga de procesar la declaracion de funciones del lenguaje
		/// </summary>
		public Node Function()
		{
			position++;

			Node operation = Global_Layer();
			Exceptions_Missing(operation, "function");
			return operation;
		}
	}
}