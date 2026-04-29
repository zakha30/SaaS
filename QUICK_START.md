# 🚀 Quick Start Guide

## Prerequisites
- .NET 10 SDK
- Node.js 18+
- SQL Server (local or remote)
- Visual Studio 2026 or VS Code

---

## ⚡ 5-Minute Setup

### 1. Backend Setup
```bash
cd SaaS.API

# Build
dotnet build

# Update database (first time)
dotnet ef database update -p ../SaaS.Infrastructure

# Run
dotnet run
```

**Backend running at:** `https://localhost:7089`

Check API: `https://localhost:7089/swagger`

### 2. Frontend Setup
```bash
cd frontend

# Install dependencies
npm install

# Start dev server
npm run dev
```

**Frontend running at:** `http://localhost:3000`

### 3. Create First Tenant & Account
1. Go to `http://localhost:3000/register`
2. Fill in:
   - First Name: "Admin"
   - Last Name: "User"
   - Email: "admin@company.com"
   - Password: "SecurePass123!"
   - Company Name: "My Transport Co"
   - Company Slug: "my-transport" (must be unique, lowercase, no spaces)
   - Select a Plan
3. Click "Create Account"
4. You'll be logged in automatically

### 4. Start Using the App
- **Dashboard:** View KPIs
- **Listings:** Create freight listings or search for loads
- **Fleet:** Add your vehicles
- **Bookings:** See quotes received

---

## 🔌 API Testing

### Via cURL
```bash
# Register
curl -X POST https://localhost:7089/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "password": "SecurePass123!",
    "tenantName": "My Company",
    "tenantSlug": "my-company-123",
    "planId": "1"
  }'

# Login
curl -X POST https://localhost:7089/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!",
    "tenantSlug": "my-company-123"
  }' \
  -c cookies.txt

# Get current user (uses cookies)
curl https://localhost:7089/api/auth/me \
  -b cookies.txt
```

### Via Postman
1. Import `SaaS.API\SaaS.API.http` into Postman
2. Set variables:
   - `baseUrl`: `https://localhost:7089`
   - `email`: Your test email
   - `password`: Your test password
3. Run requests in order

### Via Browser
Go to: `https://localhost:7089/swagger`

Interactive API documentation!

---

## 📝 Common Tasks

### Create a Listing
```
POST /api/listings
Content-Type: application/json

{
  "title": "Electronics shipment to Cape Town",
  "description": "Fragile items",
  "listingType": "OneWay",
  "vehicleType": "Van",
  "locationFrom": "Johannesburg",
  "locationTo": "Cape Town",
  "budget": 5000,
  "weight": 500,
  "availableFrom": "2024-01-20T10:00:00Z",
  "availableTo": "2024-01-22T10:00:00Z"
}
```

### Submit a Quote
```
POST /api/quotes
Content-Type: application/json

{
  "listingId": "12345678-1234-1234-1234-123456789012",
  "price": 4500,
  "message": "I can deliver within 24 hours",
  "validUntil": "2024-01-19T10:00:00Z"
}
```

### Search Listings
```
GET /api/listings/search?keyword=electronics&locationFrom=Johannesburg&maxPrice=6000
```

### Get Dashboard Summary
```
GET /api/dashboard/summary
```

---

## 🔧 Troubleshooting

### Issue: "Cannot connect to database"
**Solution:**
```bash
# Check connection string in appsettings.json
# Update if needed:
"DefaultConnection": "Server=localhost;Database=TransHub;Trusted_Connection=true;"

# Then run migrations:
dotnet ef database update -p ../SaaS.Infrastructure
```

### Issue: CORS errors on frontend
**Solution:**
- Ensure `appsettings.Development.json` includes:
  ```json
  "AllowedOrigins": ["http://localhost:3000"]
  ```
- Backend must be running on correct port

### Issue: Frontend can't login
**Solution:**
1. Check browser DevTools → Network tab
2. Look for `/api/auth/login` request
3. Check response status (should be 200)
4. Check if cookies are being set (Application tab → Cookies)

### Issue: "npm install" fails
**Solution:**
```bash
# Clear cache
npm cache clean --force

# Remove node_modules and lock
rm -rf node_modules package-lock.json

# Reinstall
npm install
```

### Issue: Vite port 3000 already in use
**Solution:**
```bash
# Use different port
npm run dev -- --port 3001
```

---

## 📱 Frontend Navigation

After login, you'll see sidebar with:
- 📊 Dashboard - KPIs
- 📋 Listings - Manage freight
- 🚚 Fleet - Your vehicles
- 👥 Drivers - Driver team
- 🚗 Trips - Trip history
- 📑 Bookings - Quote management
- 📈 Reports - Analytics
- 🔔 Notifications - Inbox

---

## 🔐 Authentication Flow

1. User enters email, password, tenant slug
2. Frontend sends POST to `/api/auth/login`
3. Backend validates credentials
4. Backend sets **HttpOnly cookies** (not in response body)
5. Frontend stores user profile in localStorage
6. Frontend redirects to `/dashboard`
7. All future API calls automatically include cookies
8. If token expires (401), frontend triggers `/api/auth/refresh`
9. New token set in cookies, request retried

**Security:** Tokens are NEVER in JavaScript, preventing XSS attacks!

---

## 📊 Database Models

### Key Entities
- **AppUser** - Users (inherits from Identity User)
- **Tenant** - Organizations/Companies
- **Plan** - Subscription plans
- **Listing** - Freight listings or vehicle hire posts
- **Quote** - Bids/offers on listings
- **Vehicle** - Fleet vehicles
- **Notification** - System notifications
- **Dashboard** - Aggregated metrics

---

## 🚀 Deployment Checklist

Before going live:

- [ ] Update `appsettings.Production.json`
- [ ] Set `AllowedOrigins` to your domain
- [ ] Use environment variables for secrets (no hardcoding!)
- [ ] Enable HTTPS only
- [ ] Set JWT Secret to >32 characters
- [ ] Configure SQL Server connection
- [ ] Run migrations on production DB
- [ ] Build frontend: `npm run build`
- [ ] Set NODE_ENV=production
- [ ] Enable HSTS headers
- [ ] Set up backups
- [ ] Monitor logs

---

## 📚 Documentation Files

- **PROJECT_STATUS.md** - Full architecture overview
- **FEATURE_GAP_ANALYSIS.md** - Missing features vs. real TransHub
- **frontend/SETUP_GUIDE.md** - Frontend detailed setup
- This file - Quick start

---

## 🎯 Your Next Goals

1. ✅ Get backend running (DONE!)
2. ✅ Get frontend running (DONE!)
3. Test login flow end-to-end
4. Create test data (listings, vehicles)
5. Test quote submission
6. Check email notifications (when configured)
7. Start building Phase 1 features:
   - Business Directory
   - Enhanced Search
   - User Profiles
   - Messaging

---

## 💬 Quick Command Reference

```bash
# Backend
dotnet build                                    # Compile
dotnet run                                      # Run API
dotnet ef migrations add MigrationName          # Create migration
dotnet ef database update                       # Apply migrations
dotnet test                                     # Run tests

# Frontend
npm install                                     # Install deps
npm run dev                                     # Dev server
npm run build                                   # Production build
npm run preview                                 # Preview build
npm run lint                                    # Check code

# Git
git status                                      # Check changes
git add .                                       # Stage all
git commit -m "message"                         # Commit
git push origin main                            # Push
```

---

## 🎓 Tips & Best Practices

1. **Always run migrations after pulling code:**
   ```bash
   dotnet ef database update -p ../SaaS.Infrastructure
   ```

2. **Use Swagger UI for API testing:**
   - Go to `https://localhost:7089/swagger`
   - Authorize with JWT token
   - Test endpoints interactively

3. **Check React DevTools (Chrome extension):**
   - Monitor state changes
   - Debug component renders
   - Check prop values

4. **Use Network tab in DevTools:**
   - Monitor API requests
   - Check response headers
   - Verify cookies are being sent
   - Check CORS headers

5. **Keep .env.local for frontend secrets:**
   ```
   VITE_API_URL=http://localhost:7089
   ```

---

## 🆘 Emergency Debug Mode

If something isn't working:

1. **Backend issues?**
   ```bash
   cd SaaS.API
   dotnet clean
   dotnet build -v detailed
   ```

2. **Frontend issues?**
   ```bash
   cd frontend
   rm -rf node_modules package-lock.json
   npm install
   npm run dev
   ```

3. **Database issues?**
   ```bash
   # Reset to fresh database
   dotnet ef database drop
   dotnet ef database update -p ../SaaS.Infrastructure
   ```

4. **Check logs:**
   - Backend: Console output during `dotnet run`
   - Frontend: Browser DevTools → Console tab
   - Database: SQL Server Management Studio

---

## ✨ Success! You're Ready!

Your TransHub platform is now set up and ready for development.

**Next:** Start building Phase 1 features from `FEATURE_GAP_ANALYSIS.md`

**Questions?** Check the documentation files or search the code!

**Happy shipping! 🚀**
