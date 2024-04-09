# E-Commerce

- ASP.NET CORE MVC 7.0
- MYSQL Database 8.0.36

### Nuget
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 7.0
- Microsoft.EntityFrameworkCore.Tools --version 7.0.4
- Microsoft.EntityFrameworkCore --version 7.0.4
- PagedList.Core.Mvc --version 3.0.0
- AspNetCoreHero.ToastNotification --version 1.1.0
- Pomelo.EntityFrameworkCore.MySql --version 7.0.0
- Microsoft.VisualStudio.Web.CodeGeneration.Design --version 7.0
- Microsoft.AspNetCore.Identity.UI (--user for AddDefaultIdentity()--)

### Model Scaffolding
- `dotnet ef dbcontext scaffold "server=127.0.0.1;uid=root;pwd=tyan;database=ecommerce;port=3306" "Pomelo.EntityFrameworkCore.MySql" -o .\Models -f`

### Pagination
- Install Package `PagedList.Core.Mvc` from Nuget

- Edit `_ViewImports.cshtml`:
    + @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
    + @addTagHelper *, PagedList.Core.Mvc
