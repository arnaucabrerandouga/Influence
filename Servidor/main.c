#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql/mysql.h>
#include <pthread.h>

int contador;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;
int sockets[100];

typedef struct{
	char Nombre[20];
	int socket;
}Tuser;

typedef struct{
	Tuser Conectados[50];
	int num;
}Tconectados;

typedef struct{
	Tuser Jugadores[4];
	int numJugadores;
	int matriz[14][14]; //filas columans
}Tpartida;

typedef struct{
	Tpartida partidas[50];
	int num;
}Tpartidas;

Tconectados conectados;
Tpartidas partidas;



int iniciar_sesion(char* nombre, char* contrasena) { //La funcion "iniciar_sesion" toma dos parametros: "nombre" y "contrasena" que son los datos de inicio de sesi√≥n del usuario que se van a verificar en la base de datos. Devuelve un valor entero que indica si el usuario existe en la base de datos o no.
	MYSQL* conexion;
	MYSQL_RES* resultado;
	MYSQL_ROW fila;
	int respuesta = 0;
	
	
	conexion = mysql_init(NULL); //Se inicializa la conexion a la base de datos utilizando la funcion "mysql_init". En caso de que la conexion no se pueda establecer, se imprime un mensaje de error y se termina el programa.
	if (mysql_real_connect(conexion, "shiva2.upc.es", "root", "mysql", "MG4_juego", 0, NULL, 0) == NULL) {
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	char consulta[1000];
	sprintf(consulta, "SELECT COUNT(*) FROM JUGADOR WHERE USERNAME='%s' AND PASSWORD='%s'", nombre, contrasena); //Esta l√≠nea construye una consulta SQL que cuenta el n√∫mero de filas en la tabla usuarios que coinciden con el nombre y la contrase√±a proporcionados. La consulta utiliza la funci√≥n sprintf para construir la consulta utilizando los valores de los par√°metros nombre y contrasena.
	
	if (mysql_query(conexion, consulta)) { //Se ejecuta la consulta utilizando la funci√≥n "mysql_query" y se verifica si hubo alg√∫n error en la ejecuci√≥n de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}


	
	resultado = mysql_store_result(conexion); //Esta linea almacena los resultados de la consulta SQL en el objeto resultado.
	fila = mysql_fetch_row(resultado); //A continuacion, se llama a la funci√≥n mysql_fetch_row() para obtener la siguiente fila de resultados. Esta funci√≥n devuelve un puntero a una estructura MYSQL_ROW que contiene los datos de una fila de resultados. Como la consulta devuelve una sola fila con un unico valor (el n√∫mero de usuarios con el nombre y contrase√±a proporcionados), no es necesario iterar sobre las filas de resultados.
	int exsiste_usuario = atoi(fila[0]); //Entonces, como esta consulta devuelve una √∫nica fila y una √∫nica columna con el n√∫mero de usuarios que cumplen la condici√≥n, se obtiene el valor de esta columna usando fila[0] y se convierte a un valor entero usando atoi(). Si el valor de existe_usuario es 0, significa que no existe un usuario con el nombre y contrase√±a proporcionados. Si el valor de existe_usuario es 1, significa que s√≠ existe un usuario con el nombre y contrase√±a proporcionados. Este valor se devuelve al final de la funci√≥n iniciar_sesion().
	
	
	if (exsiste_usuario == 0)
	{
		respuesta = 1;

	}
	else if (exsiste_usuario == 1)
	{
		respuesta = 2;
	}
	
	mysql_free_result(resultado);
	mysql_close(conexion);

	return respuesta;
}
///--------------------------------------------------------------------------------------------------
int registrarse(char* usuario, char* contrasena)
{
	MYSQL* conexion = mysql_init(NULL); // inicializar objeto de conexiÛn a MySQL
	int resultado = 0; // variable para almacenar el resultado de la operaciÛn
	
	if (conexion == NULL) // comprobar si la inicializaciÛn de la conexiÛn fue exitosa
	{
		printf("Error al inicializar la conexiÛn a MySQL: %s\n", mysql_error(conexion));
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	// conectar a la base de datos
	if (mysql_real_connect(conexion, "shiva2.upc.es", "root", "mysql", "MG4_juego", 0, NULL, 0) == NULL)
	{
		printf("Error al conectar a la base de datos: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	// crear consulta para saber cuantos jugadores hay
	char consulta0[200];
	sprintf(consulta0, "SELECT COUNT(*) FROM JUGADOR");
	// ejecutar consulta
	if (mysql_query(conexion, consulta0))
	{
		printf("Error al ejecutar la consulta0: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	MYSQL_RES* resultados0 = mysql_use_result(conexion);
	if (resultados0 == NULL)
	{
		printf("Error al obtener los resultados de la consulta0: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	MYSQL_ROW fila0 = mysql_fetch_row(resultados0);
	int num_filas0 = mysql_num_rows(resultados0);
	int num_jugadores = atoi(fila0[0]);
	
	mysql_free_result(resultados0); // liberar la memoria de los resultados de la consulta1
	
	// crear consulta SQL para comprobar si el usuario ya existe en la base de datos
	char consulta1[200];
	sprintf(consulta1, "SELECT COUNT(*) FROM JUGADOR WHERE USERNAME='%s'", usuario);
	
	// ejecutar la consulta1: Luego se ejecuta la consulta utilizando la funci√≥n mysql_query, y si ocurre alg√∫n error en la ejecuci√≥n, se muestra un mensaje de error, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado, que es 0.
	if (mysql_query(conexion, consulta1))
	{
		printf("Error al ejecutar la consulta1: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	// obtener los resultados de la consulta1: Si la consulta se ejecuta correctamente, se obtienen los resultados utilizando la funci√≥n mysql_use_result, que devuelve un puntero a una estructura MYSQL_RES que contiene los resultados de la consulta. Si ocurre alg√∫n error al obtener los resultados, se muestra un mensaje de error, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado.
	MYSQL_RES* resultados1 = mysql_use_result(conexion);
	if (resultados1 == NULL)
	{
		printf("Error al obtener los resultados de la consulta1: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	// leer el valor de la columna COUNT(*) del primer registro de la tabla de resultados
	MYSQL_ROW fila1 = mysql_fetch_row(resultados1); //La l√≠nea MYSQL_ROW fila1 = mysql_fetch_row(resultados1); es la que lee la primera fila de resultados de la consulta que se ejecut√≥ previamente. Esta fila contiene un conjunto de valores de la tabla que se corresponden con la consulta realizada.La funci√≥n mysql_fetch_row devuelve un puntero a un array de cadenas que representan los valores de la fila actual. Cada elemento del array representa un campo de la tabla y el orden de los elementos corresponde al orden de los campos en la tabla.
	if (fila1 == NULL)
	{
		printf("Error al leer los resultados de la consulta1: %s\n", mysql_error(conexion));
		mysql_free_result(resultados1);
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	int num_filas1 = mysql_num_rows(resultados1);
	int num_campos1 = mysql_num_fields(resultados1);
	int valor1 = atoi(fila1[0]); // A continuaci√≥n, se convierte el valor obtenido de la columna COUNT(*) a un entero utilizando la funci√≥n atoi. Si el valor obtenido es mayor que cero, entonces el usuario ya existe en la base de datos, se muestra un mensaje indicando que el usuario ya existe, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado.
	
	// comprobar si el usuario ya existe en la base de datos
	if (valor1 > 0) 
	{
		printf("El usuario ya existe en la base de datos\n");
		mysql_free_result(resultados1);
		mysql_close(conexion);
		resultado = 1;
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	else{
		mysql_free_result(resultados1);  // liberar la memoria de los resultados de la consulta1
		
	}
	
	if (resultado != 1)
	{
		// crear consulta SQL para insertar el nuevo usuario y la contraseÒa en la base de datos
		char consulta2[200];
		sprintf(consulta2, "INSERT INTO JUGADOR (ID, USERNAME, PASSWORD) VALUES ('%d', '%s', '%s')",num_jugadores, usuario, contrasena);
		
		// ejecutar la consulta2
		if (mysql_query(conexion, consulta2)) //el error esta aqui aun que la consulata funciona
		{
			printf("Error al ejecutar la consulta2: %s\n", mysql_error(conexion));
			mysql_free_result(resultados1);
			mysql_close(conexion);
			return resultado; // devolver valor predeterminado de resultado (0)
		}
		else
			resultado = 2; // asignar 2 a la variable resultado para indicar que el registro se realizÛ correctamente
		
		// En el cÛdigo que mostraste, no hay un else despuÈs del if porque no es necesario en este caso. Si la funciÛn mysql_query() devuelve un valor diferente de 0, se asume que hubo un error al ejecutar la consulta, por lo que el cÛdigo dentro del if se ejecuta y devuelve el valor predeterminado de resultado (0).
		// Si la funciÛn mysql_query() devuelve un valor igual a 0, se asume que la consulta se ejecutÛ correctamente, por lo que el cÛdigo dentro del if no se ejecuta y se contin˙a con el resto del cÛdigo. En este caso, no es necesario un else porque no hay ninguna acciÛn adicional que realizar si la consulta se ejecuta correctamente.
		
		//mysql_free_result(resultados1); // liberar la memoria de los resultados de la consulta1
		mysql_close(conexion); // cerrar la conexiÛn a la base de datos
		
		return resultado; // devolver el valor de la variable resultado (1 si el registro se realizÛ correctamente, 0 si hubo alg˙n error)
	}
}
///--------------------------------------------------------------------------------------
int borrarUsuario(char *usuario)
{
	MYSQL* conexion;
	MYSQL_RES* resultado;
	MYSQL_ROW fila;
	int respuesta = -1;
	
	conexion = mysql_init(NULL); //Se inicializa la conexion a la base de datos utilizando la funcion "mysql_init". En caso de que la conexion no se pueda establecer, se imprime un mensaje de error y se termina el programa.
	if (mysql_real_connect(conexion, "shiva2.upc.es", "root", "mysql", "MG4_juego", 0, NULL, 0) == NULL) {
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	//Selecciono la posicion de el jugador que voy a eliminar
	char consulta1[200];
	sprintf(consulta1, "SELECT ID FROM JUGADOR WHERE USERNAME='%s'", usuario);
	if (mysql_query(conexion, consulta1)) { //Se ejecuta la consulta utilizando la funcion "mysql_query" y se verifica si hubo algun error en la ejecuci√≥n de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return -1;
		exit(1);
	}
	MYSQL_RES* resultado1 = mysql_use_result(conexion);
	fila = mysql_fetch_row(resultado1);
	int IDEliminado = atoi(fila[0]);
	mysql_free_result(resultado1);
	
	//Elimino al jugador
	char consulta2[200];
	sprintf(consulta2, "DELETE FROM JUGADOR WHERE USERNAME='%s'", usuario);
	
	if (mysql_query(conexion, consulta2)) { //Se ejecuta la consulta utilizando la funcion "mysql_query" y se verifica si hubo algun error en la ejecuci√≥n de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return -1;
		exit(1);
	}
	else
	{
		return 1;
	}
	
	//Reorganiza la posicion de los demas jugadores
	char consulta3[200];
	sprintf(consulta3, "UPDATE JUGADOR SET ID = ID -1 WHERE ID > %d", IDEliminado);
	if (mysql_query(conexion, consulta3)) { //Se ejecuta la consulta utilizando la funcion "mysql_query" y se verifica si hubo algun error en la ejecuci√≥n de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return -1;
		exit(1);
	}

	//mysql_free_result(resultado);
	mysql_close(conexion);
}
///----------------------------------------------------------------------------------
void notificarConectados()
{
	char notificacion[20];
	sprintf(notificacion, "7/%d", conectados.num);
	int cont7 = 0;
	while (cont7 < conectados.num)
	{
		sprintf(notificacion, "%s/%s",notificacion,conectados.Conectados[cont7].Nombre);
		cont7++;
	}
	int j;
	for(j=0; j<conectados.num;j++)
	{
		printf("Notificacion: %s\n",notificacion);
		write (conectados.Conectados[j].socket,notificacion, strlen(notificacion));
	}	
}
///----------------------------------------------------------------------------------
void asignarJugadores(char notificacion[20], int numPartida, int numConectados, int i)
{
	int cont = 0;
	int encontrado = 0;
	while (cont < numConectados && encontrado == 0) //conectados.num puede fallar si se desconecta una persona
	{
		if (strcmp(partidas.partidas[numPartida].Jugadores[i].Nombre,conectados.Conectados[cont].Nombre)==0) // busco al jugador en la lista de conectados
		{
			write (conectados.Conectados[cont].socket,notificacion, strlen(notificacion));
			pthread_mutex_lock(&mutex);
			partidas.partidas[numPartida].Jugadores[i].socket = conectados.Conectados[cont].socket;
			pthread_mutex_unlock(&mutex);
			encontrado = 1;
			printf("Notificacion 9 enviada a %s \n", conectados.Conectados[cont].Nombre); // print para comprovar que se envia bien
		}
		cont=cont+1;
	}
}
///----------------------------------------------------------------------------------
rellenarMatriz(int miPartida)
{
	int x, y;
	for (x = 0; x < 14; x++) {
		for (y = 0; y < 14; y++) {
			pthread_mutex_lock(&mutex);
			partidas.partidas[partidas.num].matriz[x][y] = -1;
			pthread_mutex_unlock(&mutex);
		}
	}
}
///----------------------------------------------------------------------------------
pintarBotones(char notificacion[512], int xboton, int yboton, int miPartida, int numjugadorAct)
{	
	int numjugadorsig;
	//botons esquerra
	//si esta ala esquerra miras label de la dreta
	if (xboton == 0 && partidas.partidas[miPartida].matriz[xboton+1][yboton] == -1)
	{
		int xlabel = xboton+1;
		int ylabel = yboton;
		if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
			partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
		{
			pthread_mutex_lock(&mutex);
			partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else
			{
				if (numjugadorAct != partidas.partidas[miPartida].numJugadores -1)
				{
					numjugadorsig = numjugadorAct + 1;
				}
				else 
				{
					numjugadorsig = 0;
				}
				sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);		
			}
	}
	//botons dreta 
	//si esta a la dreta miras els labels de la esquerra
	else if (xboton == 14 && partidas.partidas[miPartida].matriz[xboton-1][yboton] == -1)
	{
		int xlabel = xboton-1;
		int ylabel = yboton;
		if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
			partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
		{
			pthread_mutex_lock(&mutex);
			partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else
			{
				if (numjugadorAct != partidas.partidas[miPartida].numJugadores -1)
				{
					numjugadorsig = numjugadorAct + 1;
				}
				else 
				{
					numjugadorsig = 0;
				}
				
				sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);		
			}
	}
	//boto de abaix
	//miras el label de asobre
	else if (yboton == 0 && partidas.partidas[miPartida].matriz[xboton][yboton+1] == -1)
	{
		int xlabel = xboton;
		int ylabel = yboton+1;
		if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
			partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
		{
			pthread_mutex_lock(&mutex);
			partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else
			{
				if (numjugadorAct != partidas.partidas[miPartida].numJugadores -1) 
				{
					numjugadorsig = numjugadorAct + 1;
				}
				else 
				{
					numjugadorsig = 0;
				}
				sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);		
			}
	}
	//botons de adalt
	// miras el label de asota
	else if (yboton == 14 && partidas.partidas[miPartida].matriz[xboton][yboton-1] == -1)
	{
		int xlabel = xboton;
		int ylabel = yboton-1;
		if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
			partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
		{
			pthread_mutex_lock(&mutex);
			partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else
			{
				if (numjugadorAct != partidas.partidas[miPartida].numJugadores -1) 
				{
					numjugadorsig = numjugadorAct + 1;
				}
				else 
				{
					numjugadorsig = 0;
				}
				sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);		
			}
	}
	//boto del mig horitzontal
	//mias label de asota i asobre
	else if (xboton % 2 == 1 && yboton % 2 == 0 )
	{
		int labelspintados = 0; // 0 no pinto nada // 1 pinto encima 2 pinto debajo 3 pinto los dos
		int xlabel;
		int ylabel;
		int xlabel2;
		int ylabel2;
		
		if (partidas.partidas[miPartida].matriz[xboton][yboton+1] == -1)
		{	
			xlabel = xboton;
			ylabel = yboton+1;
			if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
				partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
			{
				pthread_mutex_lock(&mutex);
				partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
				pthread_mutex_unlock(&mutex);
				labelspintados ++;
			}
		}
		if (partidas.partidas[miPartida].matriz[xboton][yboton-1] == -1)
		{
			xlabel2 = xboton;
			ylabel2 = yboton-1;
			if (partidas.partidas[miPartida].matriz[xlabel2+1][ylabel2] != -1 && partidas.partidas[miPartida].matriz[xlabel2-1][ylabel2] != -1 &&
				partidas.partidas[miPartida].matriz[xlabel2][ylabel2+1] != -1 && partidas.partidas[miPartida].matriz[xlabel2][ylabel2-1] != -1)
			{
				pthread_mutex_lock(&mutex);
				partidas.partidas[miPartida].matriz[xlabel2][ylabel2] = numjugadorAct;
				pthread_mutex_unlock(&mutex);
				labelspintados ++;
				labelspintados ++;
			}
		}
		if (labelspintados == 0)
		{
			if (partidas.partidas[miPartida].numJugadores -1 == numjugadorAct)
			{
				numjugadorsig = 0;
			}
			else
			{
				numjugadorsig = numjugadorAct + 1;
			}
			
			sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);	
		}
		else if (labelspintados == 1)
		{
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else if (labelspintados == 2)
		{
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel2,ylabel2);
		}
		else if (labelspintados == 3)
		{
			sprintf(notificacion, "%s/%d/2/%d/%d/%d/%d",notificacion,numjugadorAct,xlabel,ylabel,xlabel2,ylabel2);
		}	
		
	}
	//boto mig vertical 
	//miras el label de la esquerra i dreta
	else if (xboton % 2 == 0 && yboton % 2 == 1)
	{
		int labelspintados = 0; // 0 no pinto nada // 1 pinto encima 2 pinto debajo 3 pinto los dos
		int xlabel;
		int ylabel;
		int xlabel2;
		int ylabel2;
		if (partidas.partidas[miPartida].matriz[xboton+1][yboton] == -1)
		{
			xlabel = xboton+1;
			ylabel = yboton;
			if (partidas.partidas[miPartida].matriz[xlabel+1][ylabel] != -1 && partidas.partidas[miPartida].matriz[xlabel-1][ylabel] != -1 &&
				partidas.partidas[miPartida].matriz[xlabel][ylabel+1] != -1 && partidas.partidas[miPartida].matriz[xlabel][ylabel-1] != -1)
			{
				pthread_mutex_lock(&mutex);
				partidas.partidas[miPartida].matriz[xlabel][ylabel] = numjugadorAct;
				pthread_mutex_unlock(&mutex);
				labelspintados ++;
			}
		}
		if (partidas.partidas[miPartida].matriz[xboton-1][yboton] == -1)
		{
			xlabel2 = xboton-1;
			ylabel2 = yboton;
			if (partidas.partidas[miPartida].matriz[xlabel2+1][ylabel2] != -1 && partidas.partidas[miPartida].matriz[xlabel2-1][ylabel2] != -1 &&
				partidas.partidas[miPartida].matriz[xlabel2][ylabel2+1] != -1 && partidas.partidas[miPartida].matriz[xlabel2][ylabel2-1] != -1)
			{
				pthread_mutex_lock(&mutex);
				partidas.partidas[miPartida].matriz[xlabel2][ylabel2] = numjugadorAct;
				pthread_mutex_unlock(&mutex);
				labelspintados ++;
				labelspintados ++;
			}
		}
		if (labelspintados == 0)
		{
			if (partidas.partidas[miPartida].numJugadores -1 == numjugadorAct)
			{
				numjugadorsig = 0;
			}
			else
			{
				numjugadorsig = numjugadorAct + 1;
			}
			sprintf(notificacion, "%s/%d/0",notificacion,numjugadorsig);	
		}
		else if (labelspintados == 1)
		{
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel,ylabel);
		}
		else if (labelspintados == 2)
		{
			sprintf(notificacion, "%s/%d/1/%d/%d",notificacion,numjugadorAct,xlabel2,ylabel2);
		}
		else if (labelspintados == 3)
		{
			sprintf(notificacion, "%s/%d/2/%d/%d/%d/%d",notificacion,numjugadorAct,xlabel,ylabel,xlabel2,ylabel2);
		}
	}
}
///----------------------------------------------------------------------------------
void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	int ret;
	
	char peticion[512];
	char respuesta[512];
	
	int finConexion = 0;
	int notificacion7;  //avisa si debo enviar una notificacion o no
	
	int mensajes10Recividos = 0;
	int mensajes10SI = 0;
	
	while (finConexion==0)
	{
		notificacion7 = 0;
		
		// Ahora recibimos el mensaje, que dejamos en buff
		ret=read(sock_conn,peticion, sizeof(peticion));
		if (ret <= 0) {
			// si hubo un error en la lectura, cerrar la conexion
			close(sock_conn);
			printf ("error al leer el mensaje\n");
			break;
		}
		printf ("Recibido\n");
		//printf ("%d",sock_conn);
		// Tenemos que aÒadirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		printf ("Peticion: %s\n",peticion);
		
		char *p = strtok(peticion, "/");
		int codigo = atoi (p);
		int numForm;
		if (codigo == 0)
			finConexion = 1;
		
		else if (codigo== 1 || codigo == 2)
		{
			//Partimos para obtener nombre y contraseÒa
			p = strtok (NULL, "/");
			char usuario[20];
			strcpy (usuario, p);
			p = strtok (NULL, "/");
			char contrasenya[20];
			strcpy (contrasenya, p);
			
			if (codigo == 1) //iniciar sesion
			{
				pthread_mutex_lock(&mutex);
				int inicioSesion = iniciar_sesion(usuario,contrasenya); // funcion que devuelve 1 se todo va bien 0 si hay error 2 si el usuario y la contraseÒa no coinciden
				pthread_mutex_unlock(&mutex);
				if (inicioSesion == 2) // El usuario y la contraaeÒa coinciden
					sprintf (respuesta, "1/BIEN");
				else if (inicioSesion == 1) // El usuario y la contraseÒa no coinciden
					sprintf(respuesta, "1/INCORRECTO");
				else // Hay un error
					sprintf(respuesta, "1/ERROR");
			}
			else if (codigo == 2) //registrar a un usuario
			{
				pthread_mutex_lock(&mutex);
				int registro = registrarse(usuario,contrasenya); // funcion que devuelve 1 se todo va bien 0 si hay error 2 si el usuario ya existe
				pthread_mutex_unlock(&mutex);
				if (registro == 2) // Se ha registrado
					sprintf (respuesta, "2/BIEN");
				else if (registro == 1) // El usuario ya existe
					sprintf(respuesta, "2/EXISTE");
				else// Error
					sprintf(respuesta, "2/ERROR");
			}
		}
		else if (codigo == 3) //consulta 1
		{		
		}
		else if (codigo == 4) //Elimino cuenta, recibo 4/jugador/contra
		{
			p = strtok (NULL, "/");
			char usuario[20];
			strcpy (usuario, p);
			p = strtok (NULL, "/");
			char contrasenya[20];
			strcpy (contrasenya, p);
			pthread_mutex_lock(&mutex);
			int inicioSesion = iniciar_sesion(usuario,contrasenya); // funcion que devuelve 1 se todo va bien 0 si hay error 2 si el usuario y la contraseÒa no coinciden
			pthread_mutex_unlock(&mutex);
			if (inicioSesion == 2) // El usuario y la contraaeÒa coinciden
			{
				int funcion4 = borrarUsuario(usuario);  //devuelve 1 si el usuario existe, 0 si no existe
				if (funcion4 == -1)
					sprintf(respuesta, "4/ERROR", numForm);
				if (funcion4 == 1)
					sprintf(respuesta, "4/ELIMINADO", numForm);
			}
			else if (inicioSesion == 1) // El usuario y la contraseÒa no coinciden
				sprintf(respuesta, "4/ContraseÒa incorrecta");
			else // Hay un error
				sprintf(respuesta, "4/ERROR");		
		}
		else if (codigo == 5) //un usuario se ha conectado
		{
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont <= conectados.num && encontrado == 0){ //conectados.num puede fallar si alguien se desconecta
				if (strlen(conectados.Conectados[cont].Nombre)==0) //busca una posicion vacia
				{
					pthread_mutex_lock(&mutex);
					strcpy (conectados.Conectados[cont].Nombre, p); //aÒade una persona a la lista de conectados
					conectados.Conectados[cont].socket = sock_conn; //asginar socket
					conectados.num++; //aumenta en 1 el numero de personas conectadas
					pthread_mutex_unlock(&mutex);
					encontrado = 1;
					printf("%s se ha conectado\n", conectados.Conectados[cont].Nombre); // print para comprar quien se ha conectado
				}
				cont=cont+1;
			}
			notificarConectados();//cada vez que alguein se conecta o desconecta se encio un /7 para notificar a todos
		}
		else if (codigo == 6) //un usuario se ha desconectado
		{
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont <= conectados.num && encontrado == 0){
				if (strcmp(p,conectados.Conectados[cont].Nombre)==0) // busco al jugador en la lista de conectados
				{
					pthread_mutex_lock(&mutex);
					conectados.Conectados[cont].Nombre[0] = '\0';  //elimino al jugador de la lista de conectados
					conectados.Conectados[cont].socket = 0;  //elimino al socket de este jugador
					conectados.num--; //reduzco en 1 el numero de personas conectadas
					pthread_mutex_unlock(&mutex);
					encontrado = 1;
					printf("%s se ha desconectado\n", conectados.Conectados[cont].Nombre); // print para comprar quien se ha conectado
				}
				cont=cont+1;
			}
			notificarConectados();//cada vez que alguein se conecta o desconecta se encio un /7 para notificar a todos
		}
		else if (codigo == 8) //busco si un jugador est conectado, recibo 8/nombre
		{
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont < conectados.num && encontrado == 0){
				if (strcmp(p,conectados.Conectados[cont].Nombre)==0) // busco al jugador en la lista de conectados
				{ // si lo encuenor envio mensa si no tambien
					encontrado = 1;
				}
				cont=cont+1;
			}
			if (encontrado == 1)
			{
				sprintf(respuesta, "8/SI");
			}
			else
			{
				sprintf(respuesta, "8/NO");
			}
		}
		else if (codigo == 9) //mensaje para invitar a jugar a otros jugadores
		{
			//recivo mensaje 9/anfitrion/num otros jugadores/jugador1/jugador2/jugador3
			//de la manera que esta echo petara a las 50 partidas
			char notificacion[20];
			p = strtok (NULL, "/");
			pthread_mutex_lock(&mutex);
			strcpy(partidas.partidas[partidas.num].Jugadores[0].Nombre,p); //el anfitrion siempre estara en la posicion 0
			partidas.partidas[partidas.num].Jugadores[0].socket = sock_conn;
			p = strtok (NULL, "/");
			partidas.partidas[partidas.num].numJugadores = atoi (p);
			partidas.partidas[partidas.num].numJugadores ++;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "9/%d", partidas.num);
						
			int i = 1;
			while (i < partidas.partidas[partidas.num].numJugadores)
			{
				p = strtok (NULL, "/");
				pthread_mutex_lock(&mutex);
				strcpy(partidas.partidas[partidas.num].Jugadores[i].Nombre,p);
				pthread_mutex_unlock(&mutex);
				asignarJugadores(notificacion, partidas.num, conectados.num, i);
				i ++;
			}
			rellenarMatriz(partidas.num);
			partidas.num ++;
		}
		else if (codigo == 10) //recivo un mensaje 10/partidas.num/SI o 10/partidas.num/NO de todos los jugadores invitados
		{
			// tengo una variable mensajes recividos que aumenta cuando recivo un mensaje
			// una variable mensajes si recividos que aumenta cuando recibo un si
			// si cuando recibo todos los mensajes que devo y todos son si envio notificacion si
			// si hay algun no enviare un no
			char notificacion[20];
			mensajes10Recividos ++;
			p = strtok (NULL, "/");
			int miPartida = atoi (p);
			p = strtok (NULL, "/");
			char respuesta10[2];
			strcpy(respuesta10, p);
			//printf("%s\n", respuesta10);
			if (strcmp(respuesta10, "SI")==0)
			{
				mensajes10SI ++;
			}
			if (mensajes10Recividos == partidas.partidas[miPartida].numJugadores - 1)
			{
				if (mensajes10Recividos == mensajes10SI)
					sprintf(notificacion, "10/%d/SI", miPartida);
				else
					sprintf(notificacion, "10/%d/NO", miPartida);
				mensajes10Recividos = 0;
				int i = 0;
				while (i < partidas.partidas[miPartida].numJugadores)
				{
					write (partidas.partidas[miPartida].Jugadores[i].socket,notificacion, strlen(notificacion));
					printf("Notificacion %s enviada a %s con socket %d \n",notificacion, partidas.partidas[miPartida].Jugadores[i].Nombre, partidas.partidas[miPartida].Jugadores[i].socket);
					i ++;
				}
			}
		}
		else if (codigo == 11) //chat: recibo mensaje 11/numPartida/Jugador que escrive/mensaje
		{
			char notificacion[512];
			p = strtok (NULL, "/");
			int miPartida = atoi (p);
			p = strtok (NULL, "/");
			char escritor[20];
			strcpy (escritor,p);
			p = strtok (NULL, "/");
			sprintf(notificacion, "11/%d/%s/%s", miPartida, escritor, p);
			int i = 0;
			while (i < partidas.partidas[miPartida].numJugadores)
			{
				write (partidas.partidas[miPartida].Jugadores[i].socket,notificacion, strlen(notificacion));
				printf("Notificacion %s enviada a %s\n",notificacion, partidas.partidas[miPartida].Jugadores[i].Nombre);
				i ++;
			}
		}
		else if (codigo == 12) //Pasa informacion de una partida, recibe 12/numpartida/jugador
		{
			p = strtok (NULL, "/");
			int miPartida = atoi (p);
			p = strtok (NULL, "/");
			int encontrado = 0;
			int n = 0; //n sera mi posicion de jugador en mi partida
			while (n < partidas.partidas[miPartida].numJugadores && encontrado == 0) //busco valor de n
			{
				if (strcmp (partidas.partidas[miPartida].Jugadores[n].Nombre, p)==0)
				{
					encontrado = 1;
				}
				n++;
			}
			n--;
			sprintf(respuesta, "12/%d/%d/%d", miPartida, partidas.partidas[miPartida].numJugadores, n);
			int i = 0;
			while (i < partidas.partidas[miPartida].numJugadores)
			{
				sprintf(respuesta, "%s/%s", respuesta, partidas.partidas[miPartida].Jugadores[i].Nombre);
				i++;
			}
		}
		else if (codigo == 13) // Recibe: 13/numPartida/numJugador/BotoX/BotoY
		// Envia 13/numPartida/jugadorAnt/xboton/yboton/jugadirSig/numlabels/xlabel/ylabel/xlabel2/ylabel2
		{
			char notificacion[512];
			p = strtok (NULL, "/");
			int miPartida = atoi (p);
			int numjugadorAct; //jugador que acaba de tirar
			p = strtok (NULL, "/");
			numjugadorAct = atoi(p);
			p = strtok (NULL, "/");
			int xboton = atoi (p);
			p = strtok (NULL, "/");
			int yboton = atoi (p);
			pthread_mutex_lock(&mutex);
			partidas.partidas[miPartida].matriz[xboton][yboton] = numjugadorAct;
			pthread_mutex_unlock(&mutex);
			sprintf(notificacion, "13/%d/%d/%d/%d",miPartida,numjugadorAct,xboton,yboton);
			pintarBotones(notificacion, xboton, yboton, miPartida, numjugadorAct);

			int i = 0;
			while (i < partidas.partidas[miPartida].numJugadores)
			{
				write (partidas.partidas[miPartida].Jugadores[i].socket, notificacion, strlen(notificacion));
				printf("Notificacion %s enviada a %s\n",notificacion, partidas.partidas[miPartida].Jugadores[i].Nombre);
				i ++;
			}	
		}
	
		if((codigo == 1)||(codigo == 2)||(codigo == 3)||(codigo == 4)||(codigo == 8)||(codigo == 12))
		{
			printf("Respuesta enviada: %s\n",respuesta);
			//Enviamos la respuesta
			write (sock_conn,respuesta, strlen(respuesta));
		}
		
		if((codigo == 5)||(codigo == 6)) //cada vez que alguein se conecta o desconecta se encio un /7 para notificar a todos
		{
			// notificar a todos los clientes conectados
			
		}
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);	
}
///--------------------------------------------------------------------------------



///----------------------------------------------------------------------------------
//using namespace std;
int main (int argc, char *argv[]) {

	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port x
	serv_adr.sin_port = htons(50009);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
	
	partidas.num = 0;
	pthread_t thread;
	i=0;

	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		
		sockets[i]= sock_conn;
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		i=i+1;
	
	}
}
