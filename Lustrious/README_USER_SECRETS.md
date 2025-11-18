Instruções rápidas para configurar a API key do Maps (recomendado: User Secrets para dev)

1) Obter a chave (Google / ORS)
- Para Google Distance Matrix: https://console.cloud.google.com/apis/credentials
- Para OpenRouteService: https://openrouteservice.org/dev/#/signup

2) User Secrets (recomendado para desenvolvimento local, não comitar a chave)
- No diretório do projeto (`Lustrious`) execute:
 - `dotnet user-secrets init`
 - `dotnet user-secrets set "OpenRouteService:ApiKey" "SUA_CHAVE_ORS_AQUI"`
 - (se usar Google como fallback) `dotnet user-secrets set "Maps:ApiKey" "SUA_CHAVE_GOOGLE_AQUI"`

3) Variável de ambiente (alternativa, útil para produção)
- Windows PowerShell: `setx MAPS_API_KEY "SUA_CHAVE_GOOGLE_AQUI"`
- Linux/macOS: `export MAPS_API_KEY="SUA_CHAVE_GOOGLE_AQUI"`
- Para ORS: `setx ORS_API_KEY "SUA_CHAVE_ORS_AQUI"` (ou export equivalente)

4) Restrições de uso (recomendado)
- No Console do provedor, restrinja a chave para as APIs necessárias (Distance Matrix / ORS Matrix / Geocoding).
- Restrinja origem por IP do servidor (em produção) ou por referrer (se for client-side).

5) Teste local
- Reinicie a aplicação e faça checkout com um endereço cadastrado.
- Verifique logs em Output/console (FreteService registra informações e erros).

6) Observações
- Nunca comite chaves no repositório.
- Para ambientes CI/CD, armazene chaves nos segredos do pipeline (Azure Key Vault, GitHub Secrets, etc.).

Se quiser, eu adiciono um `dotnet user-secrets` helper no `README.md` principal do projeto ou crio um script de exemplo para Windows/Unix.
