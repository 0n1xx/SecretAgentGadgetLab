-- One-time: require existing password accounts to confirm email.
UPDATE AspNetUsers
SET EmailConfirmed = 0
WHERE PasswordHash IS NOT NULL
  AND EmailConfirmed = 1;
