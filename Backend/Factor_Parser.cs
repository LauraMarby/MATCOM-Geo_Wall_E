using G_Wall_E;
namespace INTERPRETE_C__to_HULK
{
	public partial class Parser
	{
		/// <summary>
		/// CAPA 0 o CAPA FACTOR 
		/// </summary>
		public Node Factor()
		{
			Token current_token = TS[position]; // Obtiene el token actual
			if (position >= TS.Count)
				throw new Exception("Unexpected end of input");

			// Si el token actual es un paréntesis izquierdo, procesa una expresión entre paréntesis
			if (current_token.Type == TokenType.L_PHARENTESYS)
			{
				position++;
				Node node = Global_Layer();
				if (position >= TS.Count && TS[position].Type != TokenType.R_PHARENTESYS)
				{
					Input_Error(" ')' Expected!");
				}
				position++;
				return node;
			}

			//Si el token actual es una llave izquierda, procesa expresiones divididas por comas y terminadas por una llave cerrada
			if (current_token.Type == TokenType.L_KEY)
			{
				position++;
				int first_pun = -1;
				bool comma = false;
				bool infinite = false;
				int inf_count = 0;
				Node node = new Node { Type = "sequence" };
				while (TS[position].Type != TokenType.R_KEY)
				{
					//if (comma || infinite) position++;
					if (TS[position].Type == TokenType.COMMA)
					{
						if (first_pun == -1) first_pun = position;
						if (position == first_pun) comma = true;
						else if (infinite) Input_Error(" Invalid sequence declaration");
						position++;
					}
					if (TS[position].Type == TokenType.INFINITE)
					{
						inf_count++;
						if (inf_count > 1) Input_Error("Invalid sequence declaration");
						if (position == first_pun) infinite = true;
						if (first_pun == -1)
						{
							first_pun = position;
							infinite = true;
						}
						else if (comma) Input_Error(" Invalid sequence declaration");
						position++;
					}
					is_in_seq = true;
					Node children = Global_Layer();
					if (!infinite) Exceptions_Missing(children, "sequence");
					if (infinite && children.Type == "error" && TS[position].Type == TokenType.R_KEY) break;
					node.Children.Add(children);
				}

				if (infinite) node.Type = "inf_sequence";
				position++;
				is_in_seq = false;
				return node;
			}

			// si el token es !
			if (current_token.Value.ToString() == "!")
			{
				string? op = Convert.ToString(TS[position++].Value);
				Node n = Global_Layer();
				return new Node { Type = op, Children = new List<Node> { n } };

			}

			//Si el token actual es un número, retorna un nodo de tipo "number" con el valor del número
			else if (TS[position].Type == TokenType.NUMBER)
			{
				dynamic value = Convert.ToDouble(TS[position++].Value);
				return new Node { Type = "number", Value = value };
			}

			//Si el token actual es "true", retorna un nodo de tipo "true" con el valor true
			else if (TS[position].Type == TokenType.TRUE)
			{
				dynamic value = TS[position++].Value;
				return new Node { Type = "true", Value = value };
			}

			// Si el token actual es "false", retorna un nodo de tipo "false" con el valor false
			else if (TS[position].Type == TokenType.FALSE)
			{
				dynamic value = TS[position++].Value;
				return new Node { Type = "false", Value = value };
			}

			// Si el token actual es una cadena, retorna un nodo de tipo "string" con el valor de la cadena
			else if (TS[position].Type == TokenType.STRING)
			{
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "string", Value = value };
			}

			//si el token actual es undefined, retorna el nodo de tipo undefined 
			else if (TS[position].Type == TokenType.UNDEFINED)
			{
				position++;
				return new Node { Type = "undefined" };
			}

			//si el token es una variable o un underscore
			else if (TS[position].Type == TokenType.VARIABLE || TS[position].Type == TokenType.UNDERSCORE)
			{
				//si a la variable le precede una coma, procesar como asignacion de valores de una secuencia
				if (TS[position + 1].Type == TokenType.COMMA && !is_in_seq && !is_param)
				{
					//crear nodo hijo con la lista de variables a asignar
					Node vars = new Node { Type = "asign_seq_var_name" };
					bool comma = true;
					do
					{
						//si encuentro un underscore, anadir al arbol
						if (TS[position].Type == TokenType.UNDERSCORE)
						{
							/*dynamic? values = Convert.ToString(TS[position++].Value);
							Node var = new Node { Type = "underscore", Value = values };
							comma = false;
							vars.Children.Add(var);*/
							position++;
							continue;
						}
						//si encuentro una variable, anadir
						else if (TS[position].Type == TokenType.VARIABLE)
						{
							dynamic? values = Convert.ToString(TS[position++].Value);
							Node var = new Node { Type = "variable", Value = values };
							comma = false;
							vars.Children.Add(var);
						}
						//si encuetro una coma y es valido, operar, sino, lanzar error
						if (TS[position].Type == TokenType.COMMA)
						{
							if (comma) Input_Error("Invalid operation in constant asignation");
							position++;
							comma = true;
						}
						//si encontre un igual y una coma le precede, error
						else if (TS[position].Type == TokenType.EQUAL)
						{
							if (comma) Input_Error("Invalid operation in constant asignation");
						}
						//si no encontro ninguna de las anteriores, retornar error
						else Input_Error("Invalid constant asignation");
					}
					while (TS[position].Type != TokenType.EQUAL);
					position++;

					//crear hijo con lista de valores a asiganr en las variables(secuencias)
					Node sequence = new Node { Type = "asign_values_seq" };
					Node node = Global_Layer();
					sequence.Children.Add(node);
					//devolver el nodo con el arbol de las asignaciones para el analisis semantico
					return new Node { Type = "sequence_asigment", Children = new List<Node>() { vars, sequence } };
				}

				//acciones que solo son validas si el token es una variable
				if (TS[position].Type == TokenType.VARIABLE)
				{
					//si a la variable le precede un igual, es de asignacion global
					if (TS[position + 1].Type == TokenType.EQUAL)
					{
						/*Node node;
						position += 2;
						if (TS[position].Type == TokenType.L_KEY) node = Factor();
						else node = Layer_0();
						return node;*/

						//este sistema vendria siendo parecido al del let in, con la unica diferencia de que cuardo exclusivamente una variable
						Node name = new Node { Type = "name", Value = TS[position].Value };
						position += 2;
						switch (TS[position].Type) //si lo que viene despues del igual son los metodos de Layer_0, llamar
						{
							case TokenType.LINE:
							case TokenType.SEGMENT:
							case TokenType.RAY:
							case TokenType.ARC:
							case TokenType.CIRCLE:
							case TokenType.MEASURE:
								return Layer_0();
							default: //si no, es un valor de variable cualquiera y lo guarda en un nodo
								Node var_value = Global_Layer();
								return new Node { Type = "global_var_asigment", Children = new List<Node> { name, var_value } };
						}
					}
					// si el token siguiente es parentesis procesar como funcion
					if (TS[position + 1].Type == TokenType.L_PHARENTESYS)
					{
						dynamic? f_name = Convert.ToString(TS[position++].Value);
						position++;
						Node name = new Node { Type = "d_function_name", Value = f_name };
						Node param = new Node { Type = "parameters" };
						if (TS[position].Type != TokenType.R_PHARENTESYS)
						{
							is_param = true;
							do
							{
								Node parammeter_name = new Node { Type = "p_name", Value = Layer_6() };
								param.Children.Add(parammeter_name);

								if (TS[position].Type == TokenType.COMMA)
								{
									position++;
								}
							} while (TS[position - 1].Type == TokenType.COMMA);
							is_param = false;
						}

						Expect(TokenType.R_PHARENTESYS, ")");
						if (TS[position].Type == TokenType.EQUAL)
						{
							Node action = Function();
							return new Node { Type = "function", Children = new List<Node> { name, param, action } };
						}
						return new Node { Type = "declared_function", Children = new List<Node> { name, param } };

					}
					// procesar como variable
					dynamic? value = Convert.ToString(TS[position++].Value);
					return new Node { Type = "variable", Value = value };
				}
				//si es un underscore, no es valido el resto de operaciones
				else
				{
					Input_Error("You're not using the underscore properly");
					return null; //no pasa nada porque siempre va a lanzar excepcion antes
				}

			}

			// Si el token actual es "cos", procesa una función coseno
			else if (TS[position].Type == TokenType.COS)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "cos", Children = new List<Node> { valor } };
			}

			// Si el token actual es "sen", procesa una función seno
			else if (TS[position].Type == TokenType.SEN)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "sin", Children = new List<Node> { valor } };
			}

			// Si el token actual es "log", procesa una función logaritmo
			else if (TS[position].Type == TokenType.LOG)
			{
				position++;
				Expect(TokenType.L_PHARENTESYS, "(");
				Node valor = Layer_6();
				Expect(TokenType.COMMA, ",");
				Node valor2 = Layer_6();
				Expect(TokenType.R_PHARENTESYS, ")");
				return new Node { Type = "log", Children = new List<Node> { valor, valor2 } };
			}

			// Si el token actual es "let", procesa una asignacion
			else if (position < TS.Count && Convert.ToString(TS[position].Value) == "let")
			{
				return Assigment();
			}

			//* GEO_WALL_E 

			else if (TS[position].Type == TokenType.POINT) //caso: point p; ó point sequence ps;
			{
				/*position++;
				Node name = Factor();
				Node x = null; //estos dos nodos null, no me queda claro que sea necesario declararlos aqui
				Node y = null; //ya que en el resto de figuras no se hace de esta manera
				//tampoco me queda claro por que en el resto de figuras se declara un expected, pero en point no se hace

				return new Node {Type = "point", Children = new List<Node>{name,x,y}};*/

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;
				if (TS[position].Type == TokenType.SEQUENCE)  //secuencia de puntos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "point_sequence", Children = new List<Node> { name } };
				}

				else //punto aleatorio
				{
					safe1 = position;
					name = Factor();
					Node x = null;
					Node y = null;
					Exceptions_Missing(name, "");

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "point", Children = new List<Node> { name, x, y } };
				}
			}

			else if (TS[position].Type == TokenType.LINE)
			{
				/*position++;
				Node name = Factor();
				position--;
				Expect(TokenType.VARIABLE, TS[position].Value);

				return new Node { Type = "line_d", Children = new List<Node> { name } };*/

				NotSaveVariable = true;
				if(is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de lineas aleatorias
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "line_sequence", Children = new List<Node> { name } };
				}

				else //linea aleatoria
				{
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					if (NotSaveVariable) //nuevo de camila
					{
						name.Value = "_" + name.Value;
						NotSaveVariable = false;
					}

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "line_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.SEGMENT)
			{
				/*position++;
				Node name = Factor();
				position--;
				Expect(TokenType.VARIABLE, TS[position].Value);

				return new Node { Type = "segment_d", Children = new List<Node> { name } };*/

				NotSaveVariable = true;
				if(is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de segmentos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "segment_sequence", Children = new List<Node> { name } };
				}

				else //segmento aleatorio
				{
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					if (NotSaveVariable) //nuevo de camila
					{
						name.Value = "_" + name.Value;
						NotSaveVariable = false;
					}

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "segment_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.RAY)
			{
				/*position++;
				Node name = Factor();
				position--;
				Expect(TokenType.VARIABLE, TS[position].Value);

				return new Node { Type = "ray_d", Children = new List<Node> { name } };*/

				NotSaveVariable = true;
				if(is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de rayos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "ray_sequence", Children = new List<Node> { name } };
				}

				else //rayo aleatorio
				{
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					if (NotSaveVariable) //nuevo de camila
					{
						name.Value = "_" + name.Value;
						NotSaveVariable = false;
					}

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "ray_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.ARC)
			{
				/*position++;
				Node name = Factor();
				position--;
				Expect(TokenType.VARIABLE, TS[position].Value);

				return new Node { Type = "arc_d", Children = new List<Node> { name } };*/

				NotSaveVariable = true;
				if(is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de arcos aleatorios
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "arc_sequence", Children = new List<Node> { name } };
				}

				else //arco aleatorio
				{
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					if (NotSaveVariable) //nuevo de camila
					{
						name.Value = "_" + name.Value;
						NotSaveVariable = false;
					}

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "arc_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.CIRCLE)
			{
				/*position++;
				Node name = Factor();
				position--;
				Expect(TokenType.VARIABLE, TS[position].Value);

				return new Node { Type = "circle_d", Children = new List<Node> { name } };*/

				NotSaveVariable = true;
				if(is_let) return Layer_0();

				position++;
				Node name;
				int safe1; //anadido por incongruhencia con position++
				int safe2;

				if (TS[position].Type == TokenType.SEQUENCE) //secuencia de circunferencias aleatorias
				{
					position++;
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");
					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "circle_sequence", Children = new List<Node> { name } };
				}

				else //circunferencia aleatoria
				{
					safe1 = position;
					name = Factor();
					Exceptions_Missing(name, "");

					if (NotSaveVariable) //nuevo de camila
					{
						name.Value = "_" + name.Value;
						NotSaveVariable = false;
					}

					safe2 = position;
					position = safe1;
					Expect(TokenType.VARIABLE, "reference");
					position = safe2;

					return new Node { Type = "circle_d", Children = new List<Node> { name } };
				}
			}

			else if (TS[position].Type == TokenType.COLOR) //si encuentro la oren de asignar color, rectificar que se le haya asigando uno y retornar nodo
			{
				int safe1;
				int safe2;
				position++;
				safe1 = position;
				Node name = Factor();
				Exceptions_Missing(name, "");
				safe2 = position;
				position = safe1;
				Expect(TokenType.COLOR_RANGE, "valid color");
				position = safe2;

				return new Node { Type = "color_asign", Children = new List<Node> { name } };
			}

			else if (TS[position].Type == TokenType.COLOR_RANGE) //si encuentro el color, asignarlo
			{
				if (position == 0 || TS[position - 1].Type != TokenType.COLOR)
				{
					Input_Error("SYNTAX ERROR: 'color' was expected before 'blue' call");
				}
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "color", Value = value };
			}

			else if (TS[position].Type == TokenType.RESTORE) //si enceuntro la orden de restore, asignarla
			{
				dynamic? value = Convert.ToString(TS[position++].Value);
				return new Node { Type = "restore", Value = value };
			}

			// Si el token actual es nulo, retorna un nodo vacío
			else if (TS[position] == null)
			{
				return new Node { };
			}

			// Si el token actual no coincide con ninguno de los anteriores, retorna un nodo de error
			else
			{
				return new Node { Type = "error", Value = 0 };
			}
		}
	}
}