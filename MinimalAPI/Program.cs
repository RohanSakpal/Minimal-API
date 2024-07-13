using Microsoft.EntityFrameworkCore;
using MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<Employee>> GetAllEmployee(DataContext context) =>
    await context.Employees.ToListAsync();

app.MapGet("/employee", async (DataContext context) => 
    await context.Employees.ToListAsync());

app.MapGet("/employee/{id}", async (DataContext context, int id) => 
    await context.Employees.FindAsync(id) is Employee emp ? 
    Results.Ok(emp) : Results.NotFound("Sorry Emp Not Found"));

app.MapPost("/employee", async (DataContext context, Employee Emp) =>
{
    context.Employees.Add(Emp);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllEmployee(context));
});

app.MapPut("/employee/{id}", async (DataContext context, Employee Emp, int id) =>
{
    var dbEmp = await context.Employees.FindAsync(id);
    if (dbEmp == null) return Results.NotFound("Sorry Emp Not Found");

    dbEmp.Firstname = Emp.Firstname;
    dbEmp.Lastname = Emp.Lastname;
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllEmployee(context));
});

app.MapDelete("/employee/{id}", async (DataContext context, int id) =>
{
    var dbEmp = await context.Employees.FindAsync(id);
    if (dbEmp == null) return Results.NotFound("Sorry Emp Not Found");

    context.Employees.Remove(dbEmp);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllEmployee(context));
});

app.Run();
