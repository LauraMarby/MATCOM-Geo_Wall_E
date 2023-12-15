using G_Wall_E;
namespace INTERPRETE_C__to_HULK
{
    public partial class Parser
    {
        //CAPAS: Estos métodos implementan la precedencia de operadores del lenguaje

		/// <summary>
		/// CAPA 6 (Operador '@' de concatenacion)
		/// </summary>
		public Node Layer_6()
		{
			Node node = Layer_5();
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "@")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_5();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		/// <summary>
		/// CAPA 5 Operadores ('&' '|')
		/// </summary>
		public Node Layer_5()
		{
			Node node = Layer_4();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "&" || Convert.ToString(TS[position].Value) == "|"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_4();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		/// <summary>
		/// CAPA 4 (Operadores '>' '<' '==' '!=' '>=' '<=' de comparacion)
		/// </summary>
		public Node Layer_4()
		{
			Node node = Layer_3();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "==" || Convert.ToString(TS[position].Value) == "!=" || Convert.ToString(TS[position].Value) == "<=" || Convert.ToString(TS[position].Value) == ">=" || Convert.ToString(TS[position].Value) == "<" || Convert.ToString(TS[position].Value) == ">"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_3();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		/// <summary>
		/// CAPA 3 (Operadores '+' suma y  '-' resta)
		/// </summary>
		public Node Layer_3()
		{
			Node node = Layer_2();
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "+" || Convert.ToString(TS[position].Value) == "-"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_2();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;

		}

		/// <summary>
		/// CAPA 2 (Operadores de '*' multiplicacion y '/' division)
		/// </summary>
		public Node Layer_2()
		{
			Node node = Layer_1();
			string? a = Convert.ToString(TS[position].Value);
			while (position < TS.Count && (Convert.ToString(TS[position].Value) == "*" || Convert.ToString(TS[position].Value) == "/" || Convert.ToString(TS[position].Value) == "%"))
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Layer_1();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		/// <summary>
		/// CAPA 1 (Operador '^' Potencia)
		/// </summary>
		public Node Layer_1()
		{
			Node node = Factor();
			while (position < TS.Count && Convert.ToString(TS[position].Value) == "^")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node right = Factor();
				Exceptions_Missing(right, "");
				node = new Node { Type = op, Children = new List<Node> { node, right } };
			}
			return node;
		}

		public Node Layer_0()
		{
			if (TS[position].Type == TokenType.LINE) //recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) //nuevo de camila
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var2 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "line", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.SEGMENT) //recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) //nuevo de camila
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var2 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "segment", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.RAY)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) //nuevo de camila
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var2 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "ray", Children = new List<Node> { name, var1, var2 } };

			}

			else if (TS[position].Type == TokenType.CIRCLE)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) //nuevo de camila
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node point = Factor();
				Expect(TokenType.COMMA, ",");
				Node measure = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "circle", Children = new List<Node> { name, point, measure } };

			}

			else if (TS[position].Type == TokenType.ARC)//recibe tres variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };

				if (NotSaveVariable) //nuevo de camila
				{
					name.Value = "_" + name.Value;
					NotSaveVariable = false;
				}

				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node var1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var2 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var3 = Factor();
				Expect(TokenType.COMMA, ",");
				Node var4 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "arc", Children = new List<Node> { name, var1, var2, var3, var4 } };
			}

			else if (TS[position].Type == TokenType.MEASURE)//recibe dos variables
			{
				Node name = new Node { Type = "g_name", Value = TS[position - 2].Value.ToString() };
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				is_param = true;
				Node point_1 = Factor();
				Expect(TokenType.COMMA, ",");
				Node point_2 = Factor();
				is_param = false;
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "measure", Children = new List<Node> { name, point_1, point_2 } };

			}

			else return null;


			//return null;
		}

    }
}