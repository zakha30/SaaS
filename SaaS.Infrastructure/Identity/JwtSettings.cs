using System;
using SaaS.Shared;

namespace SaaS.Infrastructure.Identity;

// JwtSettings has been moved to SaaS.Shared to avoid circular dependencies
// and allow it to be referenced by both Infrastructure and Auth modules.
// This file is kept for backwards compatibility - all code should reference SaaS.Shared.JwtSettings
