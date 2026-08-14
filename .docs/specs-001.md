# SDD 001 — Qute.Directus: ergonomia de declarações e API fluente de pesquisa

Status: **proposta para validação — nenhuma linha de código foi alterada por este documento.**

## 1. Contexto e objetivo

`Qute.Directus` (`Qute.Directus\Qute.Directus.csproj`, hoje v10.2.0) é um pacote NuGet que expõe a API REST do Directus 12 para qualquer aplicação C# usar como backend. Este documento avalia três eixos pedidos:

1. O projeto está bem estruturado?
2. As declarações (modelos de coleção, registro de serviços) são fáceis de usar?
3. O fluente de pesquisa (`QueryParameters`) é bom o suficiente para personalizar buscas?

Mais um ponto de atenção explícito: no Directus 12 a convenção padrão para novas coleções passou a ser o campo booleano `archived`, substituindo o antigo campo string `status` usado em coleções pré-12.

Todas as sugestões abaixo são propostas — cada uma deve receber uma decisão explícita (aceitar / rejeitar / ajustar) antes de qualquer implementação.

## 2. Diagnóstico do estado atual

### 2.1 Pontos fortes (estrutura)

- Separação limpa por responsabilidade: `Models/`, `Services/` (30 serviços cobrindo praticamente toda a API REST do Directus — items, users, files, flows, schema, etc.), `Http/`, `Serialization/`, `Extensions/`.
- Todos os modelos são `record` com propriedades `init`-only — imutável e idiomático.
- `DirectusJsonOptions` (`Qute.Directus\Serialization\DirectusJsonOptions.cs`) usa `JsonNamingPolicy.SnakeCaseLower` + `PropertyNameCaseInsensitive = true`. Isso significa que um `record` C# em PascalCase mapeia automaticamente para o snake_case do Directus **sem precisar de `[JsonPropertyName]`** em cada propriedade — confirmado no próprio `DirectusItem<TKey>` (`Qute.Directus\Models\Items\DirectusItem.cs:14-19`, onde `DateCreated`↔`date_created`, `UserUpdated`↔`user_updated` funcionam sem atributo). Isso é um ganho de ergonomia real e não trivial.
- Erros centralizados e informativos: todo `HttpResponseMessage` não-2xx passa por `EnsureSuccessAsync` (`Qute.Directus\Http\DirectusHttpClient.cs:297-321`), que tenta parsear o `{ "errors": [...] }` estruturado do Directus e lança `DirectusException` com a lista de erros (mensagem, código, extensions). Todos os 30 serviços passam exclusivamente por `DirectusHttpClient` — nenhum monta `HttpRequestMessage` por fora — então o comportamento de erro é consistente em todo o pacote.
- `AddDirectus(...)` (`Qute.Directus\Extensions\ServiceCollectionExtensions.cs`) registra client, `TokenManager` e `HttpClient` (via `IHttpClientFactory`) em uma chamada.
- **A convenção Directus 12 já está implementada**: `QueryParameters.NotArchived()` / `OnlyArchived()` / `WithStatus()` (`Qute.Directus\Models\QueryParameters.cs:58-68`) e o `README.md:36-39` documenta explicitamente a diferença entre coleções pré-12 (`status`) e 12+ (`archived`). **Isso não é um gap** — é o ponto mais forte do pacote em relação ao pedido original. O gap real, descrito em 3, é que nenhum consumidor real usa isso ainda.
- Cancellation tokens presentes em praticamente todo método assíncrono público.
- XML doc comments presentes na maioria dos membros públicos — acima da média para um pacote deste porte.

### 2.2 Gap central — API fluente de pesquisa

`QueryParameters.Where(field, value)` (`Qute.Directus\Models\QueryParameters.cs:52-53`) só produz o operador `_eq`:

```csharp
public QueryParameters Where(string field, object? value)
    => Filter(new Dictionary<string, object?> { [field] = new Dictionary<string, object?> { ["_eq"] = value } });
```

Não existe nenhum atalho fluente para os demais operadores de filtro do Directus: `_neq`, `_gt`, `_gte`, `_lt`, `_lte`, `_in`, `_nin`, `_contains`, `_icontains`, `_starts_with`, `_ends_with`, `_null`, `_nnull`, `_empty`, `_nempty`, `_between`. Para qualquer busca além de "campo igual a valor", o consumidor precisa montar um `Dictionary` cru e passar em `.Filter(object)` — o que na prática anula a proposta de ser "fluente" assim que a busca fica um pouco mais sofisticada. Também não há como expressar `_or` — `Filter`/`Where` sempre combinam tudo em AND.

### 2.3 Gap central — declarações fáceis de usar

`ItemsService` (`Qute.Directus\Services\ItemsService.cs`) não é vinculado a uma coleção: todo método recebe `string collection` como primeiro argumento e o tipo do item como parâmetro genérico separado:

```csharp
await client.Items.GetManyAsync<Article>("articles", q => ...);
```

Nada amarra `Article` a `"articles"` — é fácil digitar a string errada, e refatorar o nome da coleção não é seguro (o compilador não ajuda). Confirmado por busca ampla no pacote: não existe nenhuma classe de atributo (`grep` por `Attribute` em `Qute.Directus\**` não retorna nenhum resultado), então não há como um modelo declarar de forma declarativa a qual coleção ele pertence.

Outro ponto menor: `DirectusListResponse<T>` (`Qute.Directus\Models\DirectusResponse.cs:15-19`) não implementa `IEnumerable<T>`/`IReadOnlyList<T>` — para iterar é sempre `response.Data`, nunca `foreach (var x in response)`.

### 2.4 Evidência de uso real (o que os 3 apps consumidores mostram)

O repositório inclui três consumidores do pacote — `Qute.Console`, `Qute.PWA`, `Qute.View` — todos referenciando `Qute.Directus` via `ProjectReference`. A forma como eles realmente usam a lib é o dado mais objetivo desta análise:

- **Nenhum dos três declara um único modelo tipado para uma coleção de conteúdo real.** Toda coleção de negócio (`municipios` em Qute.Console, `acoes` em Qute.PWA) é consumida como `JsonElement` cru, com `TryGetProperty`/`ValueKind` manuais em vez de `GetManyAsync<T>`/`GetByIdAsync<T>` com um `record` de verdade. O único modelo tipado tocado em qualquer um dos três apps é `DirectusUser`, que já vem pronto no próprio pacote.
- Em `Qute.PWA\Pages\Home.razor:121-124` há código morto/comentado com duas tentativas de usar o builder fluente — uma delas citando `NotArchived()` — abandonadas em favor de `QueryParameters.FromRaw("fields=id,nome")`:

  ```csharp
  // _json = await Directus.Items.GetByIdAsync("acoes", "300211", q => q.Fields("*,municipio.*"));
  // _json = await Directus.Items.GetByIdAsync("acoes", "300211", q => q.Raw("fields=id,nome"));
  // Directus 12+: q => q.Fields("*,municipio.*").NotArchived() exclui itens arquivados (campo booleano "archived")
  _json = await Directus.Items.GetByIdAsync("acoes", "300211", QueryParameters.FromRaw("fields=id,nome"));
  ```

  Um desenvolvedor real recuou para query string crua mesmo num caso trivial de selecionar dois campos — um sinal concreto de fricção/falta de confiança no builder fluente, mesmo antes de qualquer filtro mais complexo entrar em cena.
- `NotArchived`/`OnlyArchived` (a convenção Directus 12) aparecem em código executável **zero vezes** nos três apps — só no comentário acima. Ou seja, o suporte já existente à convenção 12 está, na prática, não testado por nenhum consumidor real.
- Boilerplate duplicado entre Qute.PWA e Qute.View: a mesma cadeia de fallback de `BaseUrl` (env var → config → `HostEnvironment.BaseAddress`) está copiada nos dois `Program.cs`; o padrão `@inject DirectusClient` + `GetCurrentAsync()` + `try/catch/finally _loading` se repete em `Home.razor`/`Profile.razor` dos dois apps sem nenhum componente/serviço base compartilhado. Isso é uma observação sobre os apps consumidores, não do pacote em si (ver seção 5).

### 2.5 Bugs / inconsistências encontrados

Não fazem parte do pedido original, mas foram encontrados durante a análise da camada HTTP/infra e são relevantes para "bem estruturado":

1. **`DirectusOptions.AutoRefreshToken` é uma opção morta.** Declarada em `Qute.Directus\DirectusOptions.cs:21` (default `true`), mas nunca lida em `TokenManager` ou `DirectusHttpClient`. Hoje, setar `AutoRefreshToken = false` não tem efeito nenhum — o refresh acontece de qualquer forma se um `_refreshFunc` estiver registrado.
2. **`TokenManager` devolve token expirado em silêncio.** Em `GetAccessTokenAsync` (`Qute.Directus\Http\TokenManager.cs:58-85`), quando o token expirou e não há `_refreshToken`/`_refreshFunc` configurado, o método retorna o token expirado como se fosse válido (linha 67-68: `if (_refreshToken is null || _refreshFunc is null) return _accessToken;`). O consumidor só descobre o problema quando o Directus devolve 401 — um erro mais difícil de diagnosticar do que necessário.
3. **`ReadListResponseAsync` inconsistente com os demais `Read*`.** Em `Qute.Directus\Http\DirectusHttpClient.cs:289-295`, se o corpo da resposta desserializar para `null` em uma chamada de lista bem-sucedida (2xx), o método devolve uma `DirectusListResponse<T>` vazia em vez de lançar. Já `ReadResponseAsync`/`ReadFullResponseAsync` (linhas 272-287) lançam `DirectusException` no mesmo cenário. Essa divergência pode mascarar uma falha real (ex.: proxy devolvendo 200 com corpo vazio) como "coleção vazia".
4. **Nomenclatura ambígua entre `PostRawAsync` e `PostAuthenticatedRawAsync`** (`Qute.Directus\Http\DirectusHttpClient.cs:144-163`). "Raw" significa duas coisas diferentes nos dois métodos: no primeiro, pula autenticação (usado só no login); no segundo, pula o unwrap do envelope `{data: T}`. Isso pode confundir quem for estender o cliente.

Não há testes automatizados no pacote (nenhum diretório/projeto de teste encontrado no repositório).

## 3. Decisões de escopo já validadas com o usuário

As três perguntas abaixo foram feitas antes de escrever este SDD, e as opções recomendadas foram confirmadas:

1. **Filtros fluentes**: seguir com métodos nomeados (`WhereGreaterThan`, `WhereIn`, `WhereContains`, `WhereNull`, `WhereBetween` etc.), na mesma convenção de `Where`/`NotArchived` já existente — **sem** builder de expressões LINQ tipadas por enquanto.
2. **Binding de coleção**: propor atributo `[DirectusCollection("nome")]` + cliente tipado (`Items.Of<T>()`) para eliminar a repetição de string de coleção + tipo genérico.
3. **Bugs de comportamento** (seção 2.5, itens 1–3): tratar como *breaking change* e propor bump para **v11.0.0**, mantendo a série v10.x livre dessas mudanças de comportamento.

## 4. Propostas

Cada proposta é marcada **Additive** (compatível com v10.x, pode entrar em v10.3.0) ou **Breaking** (requer v11.0.0).

### 4.1 Operadores de filtro nomeados — *Additive*

Adicionar a `QueryParameters` (`Qute.Directus\Models\QueryParameters.cs`) um método privado `Op` (refatorando `Where` para usá-lo) e uma família de métodos públicos, um por operador Directus:

```csharp
private QueryParameters Op(string field, string directusOperator, object? value)
    => Filter(new Dictionary<string, object?> { [field] = new Dictionary<string, object?> { [directusOperator] = value } });

public QueryParameters Where(string field, object? value) => Op(field, "_eq", value);
public QueryParameters WhereNot(string field, object? value) => Op(field, "_neq", value);
public QueryParameters WhereGreaterThan(string field, object value) => Op(field, "_gt", value);
public QueryParameters WhereGreaterThanOrEqual(string field, object value) => Op(field, "_gte", value);
public QueryParameters WhereLessThan(string field, object value) => Op(field, "_lt", value);
public QueryParameters WhereLessThanOrEqual(string field, object value) => Op(field, "_lte", value);
public QueryParameters WhereIn(string field, params object[] values) => Op(field, "_in", values);
public QueryParameters WhereNotIn(string field, params object[] values) => Op(field, "_nin", values);
public QueryParameters WhereContains(string field, string value) => Op(field, "_contains", value);
public QueryParameters WhereIContains(string field, string value) => Op(field, "_icontains", value);
public QueryParameters WhereStartsWith(string field, string value) => Op(field, "_starts_with", value);
public QueryParameters WhereEndsWith(string field, string value) => Op(field, "_ends_with", value);
public QueryParameters WhereNull(string field) => Op(field, "_null", true);
public QueryParameters WhereNotNull(string field) => Op(field, "_nnull", true);
public QueryParameters WhereEmpty(string field) => Op(field, "_empty", true);
public QueryParameters WhereNotEmpty(string field) => Op(field, "_nempty", true);
public QueryParameters WhereBetween(string field, object from, object to) => Op(field, "_between", new[] { from, to });

/// Escape hatch para operadores do Directus ainda não cobertos por um método nomeado.
public QueryParameters WhereOperator(string field, string directusOperator, object? value) => Op(field, directusOperator, value);
```

Exemplo de uso:

```csharp
var recentes = await client.Items.GetManyAsync<Article>("articles", q => q
    .NotArchived()
    .WhereGreaterThanOrEqual("date_created", DateTime.UtcNow.AddDays(-30))
    .WhereIn("category", "news", "tech")
    .Sort("-date_created"));
```

Todos combinam em AND com qualquer outro `Filter`/`Where`, exatamente como hoje.

### 4.2 Agrupamento `_or` — *Additive*

```csharp
/// Combina as condições construídas dentro de <paramref name="configure"/> com OR lógico
/// entre si; o grupo resultante é então combinado via AND com o restante do filtro.
public QueryParameters Or(Action<QueryParameters> configure)
{
    var sub = new QueryParameters();
    configure(sub);
    if (sub._filters is { Count: > 0 })
        Filter(new Dictionary<string, object> { ["_or"] = sub._filters });
    return this;
}
```

Exemplo:

```csharp
var q = new QueryParameters()
    .Where("category", "news")
    .Or(o => o.Where("featured", true).WhereGreaterThan("views", 1000));
// => { "_and": [ {"category":{"_eq":"news"}}, {"_or":[{"featured":{"_eq":true}}, {"views":{"_gt":1000}}]} ] }
```

### 4.3 Binding tipado de coleção — *Additive*

Novo atributo em `Qute.Directus.Models`:

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class DirectusCollectionAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
```

Novo método em `ItemsService` (`Qute.Directus\Services\ItemsService.cs`) e um novo tipo `DirectusCollectionClient<T>`:

```csharp
public DirectusCollectionClient<T> Of<T>() where T : class
{
    var attr = typeof(T).GetCustomAttribute<DirectusCollectionAttribute>()
        ?? throw new InvalidOperationException(
            $"'{typeof(T).Name}' has no [DirectusCollection] attribute. " +
            $"Either add one or use the string-based ItemsService methods directly.");
    return new DirectusCollectionClient<T>(this, attr.Name);
}
```

```csharp
public sealed class DirectusCollectionClient<T>(ItemsService items, string collection)
{
    public Task<DirectusListResponse<T>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => items.GetManyAsync<T>(collection, query, ct);
    public Task<DirectusListResponse<T>> GetManyAsync(Action<QueryParameters> configure, CancellationToken ct = default)
        => items.GetManyAsync<T>(collection, configure, ct);
    public Task<T> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => items.GetByIdAsync<T>(collection, id, query, ct);
    public Task<T> CreateAsync(object item, QueryParameters? query = null, CancellationToken ct = default)
        => items.CreateAsync<T>(collection, item, query, ct);
    public Task<T> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => items.UpdateAsync<T>(collection, id, data, query, ct);
    public Task DeleteAsync(string id, CancellationToken ct = default)
        => items.DeleteAsync(collection, id, ct);
}
```

Modelo e uso:

```csharp
[DirectusCollection("articles")]
public record Article : DirectusItem
{
    public string? Title { get; init; }
    public bool? Archived { get; init; }
}

var articles = await client.Items.Of<Article>()
    .GetManyAsync(q => q.NotArchived().WhereGreaterThan("views", 100));
```

A API antiga baseada em string continua existindo sem alteração — necessária para os casos dinâmicos/`JsonElement` (como em `Qute.Console`).

### 4.4 `DirectusListResponse<T>` iterável — *Additive*

```csharp
public record DirectusListResponse<T> : IReadOnlyList<T>
{
    public List<T> Data { get; init; } = [];
    public ResponseMeta? Meta { get; init; }

    public int Count => Data.Count;
    public T this[int index] => Data[index];
    public IEnumerator<T> GetEnumerator() => Data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

`Data`/`Meta` continuam existindo como estão; isso só adiciona a capacidade de `foreach (var item in response)` / `response.Count` / `response[i]` diretamente na resposta.

### 4.5 `ArchivableDirectusItem<TKey>` — *Additive*

Base opcional em `Qute.Directus\Models\Items\`, ao lado de `DirectusItem<TKey>`:

```csharp
/// <summary>
/// Como <see cref="DirectusItem{TKey}"/>, mas também traz o campo booleano <c>archived</c>
/// da convenção Directus 12+. Use quando a coleção usar essa convenção
/// (ver <see cref="QueryParameters.NotArchived"/>/<see cref="QueryParameters.OnlyArchived"/>).
/// </summary>
public abstract record ArchivableDirectusItem<TKey> : DirectusItem<TKey>
{
    public bool? Archived { get; init; }
}

public abstract record ArchivableDirectusItem : ArchivableDirectusItem<string>;
```

Uso:

```csharp
[DirectusCollection("articles")]
public record Article : ArchivableDirectusItem
{
    public string? Title { get; init; }
}
```

`DirectusItem<TKey>` permanece inalterado — esta é uma base alternativa, não uma substituição.

### 4.6 Correções de comportamento — *Breaking, propor v11.0.0*

Os quatro itens da seção 2.5, cada um com decisão independente:

1. **Honrar `AutoRefreshToken`.** Passar a flag para `TokenManager` (via construtor ou `DirectusHttpClient`) e, quando `false`, `GetAccessTokenAsync` nunca chama `_refreshFunc` — devolve o token atual (ou `null`) e deixa o consumidor lidar com o 401.
2. **`TokenManager` lança em vez de devolver token expirado em silêncio.** Quando o token expirou e não há `_refreshToken`/`_refreshFunc`, lançar `DirectusException(HttpStatusCode.Unauthorized, "Access token expired and no refresh is configured.")` em vez de retornar o token expirado.
3. **`ReadListResponseAsync` consistente com `ReadResponseAsync`/`ReadFullResponseAsync`.** Lançar `DirectusException` em corpo nulo de resposta 2xx, em vez de devolver lista vazia silenciosamente.
4. **(Opcional, prioridade menor) Renomear `PostRawAsync` → `PostUnauthenticatedAsync`** para deixar claro que a diferença para `PostAuthenticatedRawAsync` é autenticação, não "raw-ness". Pode ser aceito separadamente dos itens 1–3.

Justificativa para v11 e não v10.x: os itens 1–3 mudam comportamento observável (exceções novas onde antes havia sucesso silencioso; `AutoRefreshToken=false` passando a ter efeito real). Consumidores existentes que dependem implicitamente do comportamento atual seriam afetados.

## 5. Fora de escopo / alternativas descartadas

- **Builder de filtro via expression trees** (ex.: `Where(x => x.Categoria == "news")`, traduzindo a árvore de expressão para os operadores do Directus). Seria mais idiomático em C#, mas é um esforço de design e implementação bem maior, e muda a forma como a lib é usada. Descartado por ora a pedido do usuário — os métodos nomeados da seção 4.1 resolvem o gap principal com risco/esforço muito menor. Pode ser revisitado depois se a versão com métodos nomeados se mostrar insuficiente na prática.
- **Permitir `null` explícito em PATCH.** Hoje `DirectusJsonOptions` usa `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull` (`Qute.Directus\Serialization\DirectusJsonOptions.cs:28`), então não é possível limpar um campo via PATCH mandando `null` explicitamente — o serializer simplesmente omite a propriedade. Isso é uma limitação real (o Directus suporta `PATCH` com `null` para limpar campos), mas está fora do escopo pedido nesta análise. Registrado aqui como item conhecido para uma futura SDD.
- **Boilerplate duplicado nos apps consumidores** (Qute.Console/PWA/View) — fallback de `BaseUrl` copiado em dois `Program.cs`, padrão `@inject` + `GetCurrentAsync()` + `_loading` repetido em três páginas Razor, arquitetura de autenticação inconsistente entre PWA (tem `DirectusAuthStateProvider`) e View (não tem nada). São observações sobre os apps, não sobre o pacote `Qute.Directus` em si, e não fazem parte desta SDD.

## 6. Plano de versionamento

| Versão | Conteúdo |
|---|---|
| **v10.3.0** | 4.1 (operadores nomeados), 4.2 (`Or`), 4.3 (`[DirectusCollection]` + `Items.Of<T>()`), 4.4 (`DirectusListResponse<T>` iterável), 4.5 (`ArchivableDirectusItem<TKey>`) — tudo aditivo, sem quebrar consumidores atuais. |
| **v11.0.0** | 4.6 — as 3 (ou 4, se o rename for aceito) correções de comportamento breaking. |

## 7. Critérios de validação

- Cada item da seção 4 deve receber uma decisão explícita (ACEITAR / REJEITAR / AJUSTAR) do usuário antes de qualquer implementação começar.
- Nenhuma mudança de código faz parte desta etapa — este documento é só a proposta.
- Ao implementar o que for aceito, cada proposta aditiva (4.1–4.5) deve manter 100% de compatibilidade com o código existente em `Qute.Console`, `Qute.PWA` e `Qute.View` (nenhum deles usa as APIs afetadas hoje, então o risco de regressão é baixo, mas a compilação dos três projetos deve ser conferida).
- Os itens de v11 (4.6) devem vir acompanhados de nota de changelog explicando a mudança de comportamento, já que são breaking.
