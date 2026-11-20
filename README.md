# UsuariosApp – Consumo de API REST con .NET MAUI (MVVM + HttpClient + SQLite)

App que consume `https://jsonplaceholder.typicode.com/users`, muestra los usuarios en un `CollectionView`,
abre un detalle (`/users/{id}`) y permite crear un usuario con `POST`.

## Cómo usar estos archivos en Visual Studio

1. **Crear el proyecto:** Archivo > Nuevo > Proyecto > **.NET MAUI App**.
   Ponle de nombre **`UsuariosApp`** (así los namespaces de estos archivos coinciden).
2. **Instalar el NuGet:** clic derecho al proyecto > *Administrar paquetes NuGet* > instalar **`sqlite-net-pcl`**.
3. **Eliminar** del proyecto `MainPage.xaml` y `MainPage.xaml.cs` (ya no se usan).
4. **Copiar** a tu proyecto, respetando carpetas:
   - `Models/Usuario.cs`
   - `Services/` (`IApiService.cs`, `ApiService.cs`, `DatabaseService.cs`, `IDialogService.cs`, `ErrorHelper.cs`)
   - `ViewModels/` (los 4 archivos)
   - `Views/` (los 6 archivos: 3 páginas XAML + su código)
   - `Rutas.cs`
5. **Reemplazar** `AppShell.xaml`, `AppShell.xaml.cs` y `MauiProgram.cs` por los de este paquete.
   (No toques `App.xaml` ni `App.xaml.cs`.)
6. Compilar y ejecutar (Android Emulator o Windows Machine).

> Si ves un error sobre el proveedor de SQLite en Android, instala también `SQLitePCLRaw.bundle_green`.

## Estructura

```
UsuariosApp/
├── Models/Usuario.cs              Id, Name, Email, Phone, Website (JSON + tabla SQLite)
├── Services/
│   ├── IApiService.cs / ApiService.cs   HttpClient: GET /users, GET /users/{id}, POST /users
│   ├── DatabaseService.cs               SQLite: caché local de usuarios
│   ├── IDialogService.cs                Mensajes al usuario desde los ViewModels
│   └── ErrorHelper.cs                   Excepción -> mensaje entendible
├── ViewModels/
│   ├── BaseViewModel.cs                 INotifyPropertyChanged + IsBusy
│   ├── UsuariosViewModel.cs             Lista, RefreshView, selección, navegación
│   ├── UsuarioDetalleViewModel.cs       Recibe el Id y consulta /users/{id}
│   └── NuevoUsuarioViewModel.cs         Formulario POST
├── Views/                               UsuariosPage, UsuarioDetallePage, NuevoUsuarioPage
├── Rutas.cs                             Nombres de rutas de Shell
├── AppShell.xaml(.cs)                   Navegación y registro de rutas
└── MauiProgram.cs                       Inyección de dependencias
```

## Dónde se cumple cada requisito

| Requisito | Dónde |
|---|---|
| API pública `/users` | `ApiService.BaseUrl` |
| Modelo `Usuario` + `ApiService` con `HttpClient` | `Models/Usuario.cs`, `Services/ApiService.cs` |
| GET + `System.Text.Json` + `CollectionView` | `ApiService.GetUsuariosAsync`, `UsuariosPage.xaml` |
| Detalle con Id como parámetro + `/users/{id}` | `UsuariosViewModel.VerDetalleAsync` (`detalle?id=3`), `UsuarioDetalleViewModel.ApplyQueryAttributes` |
| Formulario POST y mostrar el Id devuelto | `NuevoUsuarioPage.xaml`, `NuevoUsuarioViewModel.EnviarAsync` |
| `async/await`, `ActivityIndicator`, `try/catch` + mensaje | `IsBusy` en los ViewModels, `ActivityIndicator` en las 3 páginas, `DialogService` |
| `RefreshView` | `UsuariosPage.xaml` |
| MVVM + servicio separado | Vistas sin lógica; `ApiService` solo habla con la API |

## Notas

- **SQLite** guarda en el dispositivo la última lista descargada. Si la API falla (sin internet), la app
  muestra esos datos y avisa al usuario.
- **JSONPlaceholder no guarda de verdad** lo que envías por POST: siempre responde con `Id = 11`.
  Es normal que luego `GET /users/11` dé error 404.
- Probado en la lógica para .NET 8, 9 y 10 (en .NET 10 `DisplayAlert` está marcado obsoleto; ya está controlado en `DialogService`).
