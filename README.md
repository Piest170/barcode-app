# Barcode

จัดการรหัสสินค้า 16 หลัก รูปแบบ xxxx-xxxx-xxxx-xxxx  
ใช้มาตรฐาน Barcode Code 39

## เทคโนโลยี
- **Backend**: C# .NET 9 Web API + EF Core + SQLite
- **Frontend**: Vue 3 + Vite + Bootstrap 5
- **Features**: เพิ่ม/ลบ/แสดง barcode/validate

## วิธีเริ่มต้นใช้งาน
### Backend
```cmd
cd backend/ProductCodeApi
dotnet run
```
### Run Test
```cmd
cd ../ProductCodeApi.Tests
dotnet test
```
### Frontend
```cmd
cd frontend
npm install
npm run dev
