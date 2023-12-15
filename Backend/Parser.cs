using System.Xml;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using G_Wall_E;

namespace INTERPRETE_C__to_HULK
{
	public partial class Parser
	{
		/// <summary>
		/// Lista de tokens
		/// </summary>
		List<Token> TS;
		///<summary>
		/// Posicion actual en la lista de tokens
		/// </summary>
		int position;
		///<summary>
		/// Diccionario de las variables
		/// </summary>
		public Dictionary<string, object> Variables;

		//booleano que me dice si una variable esta dentro de una sequencia o no
		bool is_in_seq;

		//booleano que me dice si la variable esta en foma de parametro en una funcion
		bool is_param;

		//booleano que dice si la variable esta dentro de una declaracion de variable en el let
		bool is_let;

		//nuevo de camila
		bool NotSaveVariable;

		/// <summary>
		/// Constructor de la clase Parser
		/// </summary>
		public Parser(List<Token> Tokens_Sequency)
		{
			position = 0; //inicializa la posicion a 0
			is_in_seq = false; //incializa en falso
			TS = Tokens_Sequency; // Almacena la secuencia de Tokens
			Variables = new Dictionary<string, object>(); // Inicializa el diccionario de variables
		}

		/// <summary>
		/// Método Parse que genera el árbol de análisis sintáctico
		/// </summary>
		/// <returns>
		/// Arbol de sintaxis AST
		/// </returns>
		public Node Parse()
		{
			List<Node> Children = new List<Node>();

			while (TS[position].Type != TokenType.EOF)
			{
				Children.Add(Global_Layer());
				Expect(TokenType.D_COMMA, ";");
			}

			return new Node { Type = "Root_of_the_tree", Children = Children };
		}

		/// <summary>
		/// Método Global_Layer que decide qué acción tomar en función del token actual
		/// </summary>
		public Node Global_Layer()
		{
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "draw")
			{
				return Drawable();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "measure")
			{
				return Measure();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "intersect")
			{
				return Intersect();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "points")
			{
				return Points();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "samples")
			{
				return Samples();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "randoms")
			{
				return Randoms();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "count")
			{
				return Count();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "let")
			{
				return Assigment();
			}

			if (position < TS.Count && Convert.ToString(TS[position].Value) == "if")
			{
				return Conditional();
			}
			if (position < TS.Count && Convert.ToString(TS[position].Value) == "import")
			{
				return Import_Code();
			}

			return Layer_6();
		}



		#region Auxiliar

		/// <summary>
		/// Método que lanza una excepción con un mensaje de error de sintaxis
		/// </summary>
		private void Input_Error(string error)
		{
			throw new Exception("SYNTAX ERROR: " + error);
		}

		/// <summary>
		///  Método que verifica si un nodo es de tipo "error" y lanza una excepción en ese caso
		/// </summary>
		private void Exceptions_Missing(Node node, string op)
		{
			if (node.Type == "error")
			{
				if (op == "")
				{
					Input_Error($"Missing expression after variable `{TS[position - 1].Value}`");
				}
				else
				{
					string msg = $"Missing expression in `{op}` after variable `{TS[position - 1].Value}`";
					Input_Error(msg);
				}
			}
		}

		/// <summary>
		/// Método que verifica si el token actual es del tipo esperado y avanza a la siguiente posición en ese caso, o lanza una excepción si no lo es
		/// </summary>
		public void Expect(TokenType tokenType, object? value)
		{
			if (TS[position].Type == tokenType)
			{
				position++;
			}
			else
			{
				Input_Error($"[{position}] `{value}` Expected! after `{TS[position - 1].Value}`,`{TS[position].Value}` was received");
			}
		}

		//nuevo de camila
		public bool IsGeometricToken()
		{
			switch(TS[position].Type)
			{
				case TokenType.LINE:
				case TokenType.SEGMENT:
				case TokenType.RAY:
				case TokenType.CIRCLE: 
				case TokenType.ARC:
					return true;

				default: 
					return false;
			}
		}


		#endregion

	}
}
