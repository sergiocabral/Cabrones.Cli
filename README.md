# Cli

Programa genérico para implementar ferramentas tipo CLI (Command Line Interface).

## Gerando o executável

Para gerar o executável, siga os passos abaixo no *Visual Studio*:

1. Defina o build como Release.
2. Faça o build do projeto.
3. Faça o build do projeto *novamente* para atualizar as DLLs embutidas no executável principal.
4. Clique com o botão direito no projeto `Cli`, faça o Publish sem modificar as configurações.

O executável e as bibliotecas serão geradas na pastas `_build`.

Obs.: A pasta vazia `_build\netcoreapp2.2` pode ser apagada. Ela só não foi porque não é possível apagar durante a publicação.

O executável final é o arquivo `Cli.exe`.
