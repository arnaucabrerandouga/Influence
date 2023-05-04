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

Tconectados conectados;


int iniciar_sesion(char* nombre, char* contrasena) { //La funcion "iniciar_sesion" toma dos parametros: "nombre" y "contrasena" que son los datos de inicio de sesión del usuario que se van a verificar en la base de datos. Devuelve un valor entero que indica si el usuario existe en la base de datos o no.
	MYSQL* conexion;
	MYSQL_RES* resultado;
	MYSQL_ROW fila;
	int respuesta = 0;
	
	
	conexion = mysql_init(NULL); //Se inicializa la conexion a la base de datos utilizando la funcion "mysql_init". En caso de que la conexion no se pueda establecer, se imprime un mensaje de error y se termina el programa.
	if (mysql_real_connect(conexion, "localhost", "root", "mysql", "juego", 0, NULL, 0) == NULL) {
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	char consulta[1000];
	sprintf(consulta, "SELECT COUNT(*) FROM JUGADOR WHERE USERNAME='%s' AND PASSWORD='%s'", nombre, contrasena); //Esta línea construye una consulta SQL que cuenta el número de filas en la tabla usuarios que coinciden con el nombre y la contraseña proporcionados. La consulta utiliza la función sprintf para construir la consulta utilizando los valores de los parámetros nombre y contrasena.
	
	if (mysql_query(conexion, consulta)) { //Se ejecuta la consulta utilizando la función "mysql_query" y se verifica si hubo algún error en la ejecución de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}


	
	resultado = mysql_store_result(conexion); //Esta linea almacena los resultados de la consulta SQL en el objeto resultado.
	fila = mysql_fetch_row(resultado); //A continuacion, se llama a la función mysql_fetch_row() para obtener la siguiente fila de resultados. Esta función devuelve un puntero a una estructura MYSQL_ROW que contiene los datos de una fila de resultados. Como la consulta devuelve una sola fila con un unico valor (el número de usuarios con el nombre y contraseña proporcionados), no es necesario iterar sobre las filas de resultados.
	int exsiste_usuario = atoi(fila[0]); //Entonces, como esta consulta devuelve una única fila y una única columna con el número de usuarios que cumplen la condición, se obtiene el valor de esta columna usando fila[0] y se convierte a un valor entero usando atoi(). Si el valor de existe_usuario es 0, significa que no existe un usuario con el nombre y contraseña proporcionados. Si el valor de existe_usuario es 1, significa que sí existe un usuario con el nombre y contraseña proporcionados. Este valor se devuelve al final de la función iniciar_sesion().
	
	
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
	MYSQL* conexion = mysql_init(NULL); // inicializar objeto de conexi�n a MySQL
	int resultado = 0; // variable para almacenar el resultado de la operaci�n
	
	if (conexion == NULL) // comprobar si la inicializaci�n de la conexi�n fue exitosa
	{
		printf("Error al inicializar la conexi�n a MySQL: %s\n", mysql_error(conexion));
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	// conectar a la base de datos
	if (mysql_real_connect(conexion, "localhost", "root", "mysql", "juego", 0, NULL, 0) == NULL)
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
	
	// ejecutar la consulta1: Luego se ejecuta la consulta utilizando la función mysql_query, y si ocurre algún error en la ejecución, se muestra un mensaje de error, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado, que es 0.
	if (mysql_query(conexion, consulta1))
	{
		printf("Error al ejecutar la consulta1: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	// obtener los resultados de la consulta1: Si la consulta se ejecuta correctamente, se obtienen los resultados utilizando la función mysql_use_result, que devuelve un puntero a una estructura MYSQL_RES que contiene los resultados de la consulta. Si ocurre algún error al obtener los resultados, se muestra un mensaje de error, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado.
	MYSQL_RES* resultados1 = mysql_use_result(conexion);
	if (resultados1 == NULL)
	{
		printf("Error al obtener los resultados de la consulta1: %s\n", mysql_error(conexion));
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	// leer el valor de la columna COUNT(*) del primer registro de la tabla de resultados
	MYSQL_ROW fila1 = mysql_fetch_row(resultados1); //La línea MYSQL_ROW fila1 = mysql_fetch_row(resultados1); es la que lee la primera fila de resultados de la consulta que se ejecutó previamente. Esta fila contiene un conjunto de valores de la tabla que se corresponden con la consulta realizada.La función mysql_fetch_row devuelve un puntero a un array de cadenas que representan los valores de la fila actual. Cada elemento del array representa un campo de la tabla y el orden de los elementos corresponde al orden de los campos en la tabla.
	if (fila1 == NULL)
	{
		printf("Error al leer los resultados de la consulta1: %s\n", mysql_error(conexion));
		mysql_free_result(resultados1);
		mysql_close(conexion);
		return resultado; // devolver valor predeterminado de resultado (0)
	}
	
	int num_filas1 = mysql_num_rows(resultados1);
	int num_campos1 = mysql_num_fields(resultados1);
	int valor1 = atoi(fila1[0]); // A continuación, se convierte el valor obtenido de la columna COUNT(*) a un entero utilizando la función atoi. Si el valor obtenido es mayor que cero, entonces el usuario ya existe en la base de datos, se muestra un mensaje indicando que el usuario ya existe, se libera la memoria utilizada y se devuelve el valor predeterminado de la variable resultado.
	
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
		// crear consulta SQL para insertar el nuevo usuario y la contrase�a en la base de datos
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
			resultado = 2; // asignar 2 a la variable resultado para indicar que el registro se realiz� correctamente
		
		// En el c�digo que mostraste, no hay un else despu�s del if porque no es necesario en este caso. Si la funci�n mysql_query() devuelve un valor diferente de 0, se asume que hubo un error al ejecutar la consulta, por lo que el c�digo dentro del if se ejecuta y devuelve el valor predeterminado de resultado (0).
		// Si la funci�n mysql_query() devuelve un valor igual a 0, se asume que la consulta se ejecut� correctamente, por lo que el c�digo dentro del if no se ejecuta y se contin�a con el resto del c�digo. En este caso, no es necesario un else porque no hay ninguna acci�n adicional que realizar si la consulta se ejecuta correctamente.
		
		
		//mysql_free_result(resultados1); // liberar la memoria de los resultados de la consulta1
		mysql_close(conexion); // cerrar la conexi�n a la base de datos
		
		return resultado; // devolver el valor de la variable resultado (1 si el registro se realiz� correctamente, 0 si hubo alg�n error)
	}
}
///--------------------------------------------------------------------------------------
int consulta_1()
{
	MYSQL* conexion;
	MYSQL_RES* resultado;
	MYSQL_ROW fila;
	int respuesta = -1;
	
	
	conexion = mysql_init(NULL); //Se inicializa la conexion a la base de datos utilizando la funcion "mysql_init". En caso de que la conexion no se pueda establecer, se imprime un mensaje de error y se termina el programa.
	if (mysql_real_connect(conexion, "localhost", "root", "mysql", "juego", 0, NULL, 0) == NULL) {
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	char consulta[200];
	sprintf(consulta, "SELECT COUNT(*) FROM JUGADOR");
	
	if (mysql_query(conexion, consulta)) { //Se ejecuta la consulta utilizando la funcion "mysql_query" y se verifica si hubo algún error en la ejecución de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	resultado = mysql_store_result(conexion); //Esta linea almacena los resultados de la consulta SQL en el objeto resultado.
	fila = mysql_fetch_row(resultado); //A continuacion, se llama a la función mysql_fetch_row() para obtener la siguiente fila de resultados. Esta función devuelve un puntero a una estructura MYSQL_ROW que contiene los datos de una fila de resultados. Como la consulta devuelve una sola fila con un unico valor (el número de usuarios con el nombre y contraseña proporcionados), no es necesario iterar sobre las filas de resultados.
	int usuarios= atoi(fila[0]); //Entonces, como esta consulta devuelve una unica fila y una unica columna con el número de usuarios que cumplen la condición, se obtiene el valor de esta columna usando fila[0] y se convierte a un valor entero usando atoi(). Si el valor de existe_usuario es 0, significa que no existe un usuario con el nombre y contraseña proporcionados. Si el valor de existe_usuario es 1, significa que sí existe un usuario con el nombre y contraseña proporcionados. Este valor se devuelve al final de la función iniciar_sesion().
	
	mysql_free_result(resultado);
	mysql_close(conexion);
	
	if (respuesta != -1);
		return usuarios;
}
///--------------------------------------------------------------------------------------
int consulta_2(char *usuario)
{
	MYSQL* conexion;
	MYSQL_RES* resultado;
	MYSQL_ROW fila;
	int respuesta = -1;
	
	
	conexion = mysql_init(NULL); //Se inicializa la conexion a la base de datos utilizando la funcion "mysql_init". En caso de que la conexion no se pueda establecer, se imprime un mensaje de error y se termina el programa.
	if (mysql_real_connect(conexion, "localhost", "root", "mysql", "juego", 0, NULL, 0) == NULL) {
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	char consulta[200];
	sprintf(consulta, "SELECT COUNT(*) FROM JUGADOR WHERE USERNAME='%s'", usuario);
	
	if (mysql_query(conexion, consulta)) { //Se ejecuta la consulta utilizando la funcion "mysql_query" y se verifica si hubo algún error en la ejecución de la consulta. Si hay un error, se imprime un mensaje de error y se termina el programa.
		fprintf(stderr, "%s\n", mysql_error(conexion));
		return respuesta;
		exit(1);
	}
	
	resultado = mysql_store_result(conexion); //Esta linea almacena los resultados de la consulta SQL en el objeto resultado.
	fila = mysql_fetch_row(resultado); //A continuacion, se llama a la función mysql_fetch_row() para obtener la siguiente fila de resultados. Esta función devuelve un puntero a una estructura MYSQL_ROW que contiene los datos de una fila de resultados. Como la consulta devuelve una sola fila con un unico valor (el número de usuarios con el nombre y contraseña proporcionados), no es necesario iterar sobre las filas de resultados.
	int usuarios= atoi(fila[0]); //Entonces, como esta consulta devuelve una única fila y una única columna con el número de usuarios que cumplen la condición, se obtiene el valor de esta columna usando fila[0] y se convierte a un valor entero usando atoi(). Si el valor de existe_usuario es 0, significa que no existe un usuario con el nombre y contraseña proporcionados. Si el valor de existe_usuario es 1, significa que sí existe un usuario con el nombre y contraseña proporcionados. Este valor se devuelve al final de la función iniciar_sesion().
	
	mysql_free_result(resultado);
	mysql_close(conexion);
	
	if (respuesta != -1);
	return usuarios;
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
	
	while (finConexion==0)
	{
		notificacion7 = 0;
		
		// Ahora recibimos el mensaje, que dejamos en buff
		ret=read(sock_conn,peticion, sizeof(peticion));
		if (ret <= 0) {
			// si hubo un error en la lectura, cerrar la conexion
			close(sock_conn);
			printf ("error al leer el mensaje");
			break;
		}
		printf ("Recibido\n");
		printf ("%d",sock_conn);
		// Tenemos que a�adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		printf ("Peticion: %s\n",peticion);
		
		char *p = strtok(peticion, "/");
		int codigo = atoi (p);
		int numForm;
		printf ("Codigo: %d\n",codigo);	
		if (codigo == 0)
			finConexion = 1;
		
		else if (codigo== 1 || codigo == 2)
		{
			//Partimos para obtener nombre y contrase�a
			p = strtok (NULL, "/");
			char usuario[20];
			strcpy (usuario, p);
			p = strtok (NULL, "/");
			char contrasenya[20];
			strcpy (contrasenya, p);
			
			if (codigo == 1) //iniciar sesion
			{
				int inicioSesion = iniciar_sesion(usuario,contrasenya); // funcion que devuelve 1 se todo va bien 0 si hay error 2 si el usuario y la contrase�a no coinciden
				if (inicioSesion == 2) // El usuario y la contraae�a coinciden
					sprintf (respuesta, "1/BIEN");
				else if (inicioSesion == 1) // El usuario y la contrase�a no coinciden
					sprintf(respuesta, "1/INCORRECTO");
				else // Hay un error
					sprintf(respuesta, "1/ERROR");
			}
			else if (codigo == 2) //registrar a un usuario
			{
				int registro = registrarse(usuario,contrasenya); // funcion que devuelve 1 se todo va bien 0 si hay error 2 si el usuario ya existe
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
			printf("empezamos peticion3");
			p = strtok (NULL, "/");
			int numForm = atoi (p);
			int consulta1 = consulta_1();
			sprintf(respuesta, "3/%d/%d", numForm, consulta1);
			printf("%d",consulta1);
			
		}
		else if (codigo == 4) //consulta 2
		{
			p = strtok (NULL, "/");
			int numForm = atoi (p);
			p = strtok (NULL, "/");
			char usuario[20];
			strcpy (usuario, p);
			int consulta2 = consulta_2(usuario); //devuelve 1 si el usuario existe, 0 si no existe
			
			if (consulta2 == 1)
				sprintf(respuesta, "4/%d/SI", numForm);
			if (consulta2 == 0)
				sprintf(respuesta, "4/%d/NO", numForm);
			
		}
		else if (codigo == 5) //un usuario se ha conectado
		{
			pthread_mutex_lock( &mutex );//no me interrumpas ahora
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont < 50 && encontrado == 0){
				if (strlen(conectados.Conectados[cont].Nombre)==0) //busca una posicion vacia
				{
					printf("%s esta en la posicion donde escribire\n", conectados.Conectados[cont].Nombre);
					strcpy (conectados.Conectados[cont].Nombre, p); //a�ade una persona a la lista de conectados
					conectados.Conectados[cont].socket = sock_conn; //asginar socket
					conectados.num++; //aumenta en 1 el numero de personas conectadas
					encontrado = 1;
					printf("%s se ha conectado\n", conectados.Conectados[cont].Nombre); // print para comprar quien se ha conectado
				}
				cont=cont+1;
			}
			pthread_mutex_unlock( &mutex );//ya puedes interrumpirme
		}
		else if (codigo == 6) //un usuario se ha desconectado
		{
			pthread_mutex_lock( &mutex );//no me interrumpas ahora
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont < 50 && encontrado == 0){
				if (strcmp(p,conectados.Conectados[cont].Nombre)==0) // busco al jugador en la lista de conectados
				{
					conectados.Conectados[cont].Nombre[0] = '\0';  //elimino al jugador de la lista de conectados
					conectados.Conectados[cont].socket = 0;  //elimino al socket de este jugador
					conectados.num--; //reduzco en 1 el numero de personas conectadas
					encontrado = 1;
					printf("%s se ha desconectado", conectados.Conectados[cont].Nombre); // print para comprar quien se ha conectado
					
				}
				cont=cont+1;
				
			}
			pthread_mutex_unlock( &mutex );//ya puedes interrumpirme
		}
		else if (codigo == 8)
		{
			int cont = 0;
			int encontrado = 0;
			p = strtok (NULL, "/");
			while (cont < conectados.num && encontrado == 0){
				if (strcmp(p,conectados.Conectados[cont].Nombre)==0) // busco al jugador en la lista de conectados
				{
					// si lo encuenor envio mensa si no tambiwn
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
		if((codigo == 1)||(codigo == 2)||(codigo == 3)||(codigo == 4)||(codigo == 8))
		{
			printf("%s\n",respuesta);
			//Enviamos la respuesta
			write (sock_conn,respuesta, strlen(respuesta));
		}
		
		if((codigo == 5)||(codigo == 6))
		{

			// notificar a todos los clientes conectados
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
				printf("Notificacion; %s\n",notificacion);
				printf ("%d",conectados.Conectados[j].socket);
				write (conectados.Conectados[j].socket,notificacion, strlen(notificacion));
			}
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
	serv_adr.sin_port = htons(9050);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
	

	pthread_t thread;
	i=0;

	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		//sock_conn es el socket que usaremos para este cliente
		
		sockets[i]= sock_conn;
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		i=i+1;
	
	}
}

























