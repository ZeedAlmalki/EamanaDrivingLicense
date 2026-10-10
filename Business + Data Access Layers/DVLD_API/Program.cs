var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الخدمات الأساسية
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. إعداد سياسة الـ CORS للسماح بالاتصال من أي مصدر (Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// 3. تهيئة بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 4. تفعيل سياسة الـ CORS (يجب أن تكون هنا قبل الـ MapControllers)
app.UseCors("AllowAll");

app.UseAuthorization();

// 5. ربط الـ Controllers
app.MapControllers();

app.Run();