1. Azure Blob Naming Conventions
Key Rules:

Allowed Characters:

Alphanumerics (a-z, A-Z, 0-9)

Hyphen (-), underscore (_), period (.), and forward slash (/) for virtual directories

URL-encode special characters like spaces (%20)

Restrictions:

Case-sensitive (but recommended to use lowercase consistently)

Max 1024 characters

Cannot end with slash (/) except for root directory

Avoid reserved URL characters: & $ @ = ? ; : + , * ' %


# Tenant-specific structure
{tenant-id}/{category}/{uuid-or-hash}.{ext}

# Example:
contoso/documents/3F2504E0-4F89-11D3-9A0C-0305E82C3301.pdf
acme/images/profile/859b39a7-075c-4a5e-8d20-cc078d3f5613.jpg