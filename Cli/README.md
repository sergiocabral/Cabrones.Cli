# Cli

Este é um programa genérico para implementar ferramentas tipo CLI,
i.e., Command Line Interface.

## Gerando o executável

Para gerar o executável, siga os passos abaixo no *Visual Studio*:

1. Defina o build como Release.
2. Faça o build do projeto.
3. Faça o build do projeto *novamente* para atualizar as DLLs embutidas no executável principal.
4. Clique com o botão direito no projeto `Cli`, faça o Publish sem modificar as configurações.

O executável e as bibliotecas serão geradas na pastas `_build`.

Obs.: A pasta vazia `_build\netcoreapp2.2` pode ser apagada. Ela só não foi porque não é possível apagar durante a publicação.

O executável final é o arquivo `Cli.exe`.

## Organização da Solution

- O projeto que gera o executável é o `Cli`.
- Os projetos iniciados com `Cli.Input. ...` são módulos responsáveis pelo entrada de texto por parte do usuário.
- Os projetos iniciados com `Cli.Output. ...` são módulos responsáveis pelo saída do texto para o usuário.
- Os projetos iniciados com `Cli.Module. ...` são usados para criar ferramentas tipo CLI.

## Dicas e ajuda sobre como usar...

O idioma de exibição é definido com base no idioma do sistema operacional.
Mas pode ser ajustado definido a variável de ambiente `CLI-LANG`. Valores possíveis:
- `en-US` para Ingles. (idioma padrão)
- `pt-BR` para Português.
