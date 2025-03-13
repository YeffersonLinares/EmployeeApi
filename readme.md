# 📌 Guía para ejecutar el servicio `EmployeeService` en Windows con Visual Studio 2017+

## **🔹 Requisitos previos**
Antes de ejecutar el proyecto, asegúrate de tener instalado lo siguiente:

1. **Visual Studio 2017 o superior**  
   - Descárgalo desde: [Visual Studio](https://visualstudio.microsoft.com/es/)
   - Durante la instalación, **selecciona el paquete de desarrollo .NET Core/.NET 5+**

2. **SDK de .NET Core o .NET 5+**  
   - Descárgalo desde: [SDK de .NET](https://dotnet.microsoft.com/en-us/download)

3. **Git (opcional, si necesitas clonar el proyecto desde un repositorio)**  
   - Descárgalo desde: [Git for Windows](https://git-scm.com/downloads)

4. **Postman o cURL (opcional, para probar la API)**  
   - Postman: [Descargar Postman](https://www.postman.com/downloads/)

---

## **1️⃣ Clonar o descargar el código fuente**
Si el código está en un repositorio **Git**, abre `cmd` o `PowerShell` y ejecuta:

```sh
git clone https://github.com/usuario/proyecto-empleados.git
```

Si no usas Git, descarga el código y descomprímelo en una carpeta.

---

## **2️⃣ Abrir el proyecto en Visual Studio**
1. Abre **Visual Studio 2017 o superior**.
2. Haz clic en **"Abrir un proyecto o solución"**.
3. Busca y selecciona el archivo `.sln` o la carpeta raíz del proyecto.

---

## **3️⃣ Configurar el entorno**
1. **Abrir el archivo `appsettings.json`**  
   - Asegúrate de que la URL de la API externa está configurada correctamente.

2. **Configurar `launchSettings.json`**  

3. **Configurar NuGet y restaurar paquetes**  
   En **Visual Studio**, abre la consola de **Administrador de paquetes NuGet** (`Herramientas > Administrador de paquetes NuGet > Consola`) y ejecuta:

   ```sh
   dotnet restore
   ```

---

## **4️⃣ Ejecutar el servicio**
### **Opción 1: Desde Visual Studio**
1. **Selecciona "IIS Express" o "EmployeeService"** en la barra de depuración.
2. Presiona `F5` para iniciar el servidor.
3. Si aparece un mensaje de Firewall, **permite el acceso**.

### **Opción 2: Desde la terminal**
1. Abre una terminal en la carpeta del proyecto (`cd C:\ruta\del\proyecto`).
2. Ejecuta:

   ```sh
   dotnet run
   ```

---

## **5️⃣ Probar el servicio**
Una vez iniciado, prueba los endpoints en **Postman** o en el navegador.

- Para obtener todos los empleados:
  ```sh
  GET http://localhost:5000/api/employees
  ```
- Para obtener un empleado por ID (ejemplo: ID `3`):
  ```sh
  GET http://localhost:5000/api/employees/3
  ```

Si usas **cURL**, prueba con:
```sh
curl -X GET http://localhost:5000/api/employees
```

---

## **6️⃣ Errores comunes y soluciones**

| 🔴 **Error**                     | 🔹 **Solución**  |
|-----------------------------------|----------------|
| `dotnet no se reconoce` | Instala el **SDK de .NET Core** |
| `La API externa no responde` | Revisa si la API `dummy.restapiexample.com` está en línea |
| `Error 500 en el servicio` | Revisa los logs en la consola de Visual Studio |

---

## **🎯 Conclusión**
Siguiendo estos pasos, deberías poder ejecutar `EmployeeService` sin problemas en **Windows + Visual Studio 2017+**.