using Dapper;
using System.Data;
using Vmi.Portal.Common;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories
{
    public class ClienteFornecedorRepository : IClienteFornecedorRepository
    {
        private readonly VmiDbContext _vmiDbContext;

        public ClienteFornecedorRepository(VmiDbContext vmiDbContext)
        {
            _vmiDbContext = vmiDbContext;
        }

        public async Task<ClienteFornecedor> BuscarPorId(Guid id)
        {
            var sql = @"
                SELECT * FROM ClientesFornecedores WHERE Id = @Id;
                SELECT * FROM Enderecos WHERE IdClienteFornecedor = @Id;
                SELECT * FROM Contatos WHERE IdClienteFornecedor = @Id;
                SELECT * FROM DadosFinanceiros WHERE IdClienteFornecedor = @Id;
            ";

            using (var multi = await _vmiDbContext.Connection.QueryMultipleAsync(sql, new { Id = id }))
            {
                var clienteFornecedor = await multi.ReadSingleOrDefaultAsync<ClienteFornecedor>();
                return clienteFornecedor;
            }
        }

        public async Task<PagedResult<ClienteFornecedor>> BuscarTodos(
         int pageNumber,
         int pageSize,
         string razaoSocial = null,
         string nomeFantasia = null,
         string cpfCnpj = null,
         int? tipoCadastro = null,
         int? tipoPessoa = null,
         bool? statusCadastro = null)
        {
            string sql = @"
                WITH ClientesPaginados AS (
                    SELECT 
                        cf.Id,
                        cf.TipoCadastro,
                        cf.TipoPessoa,
                        cf.TipoEmpresa,
                        cf.PorteEmpresa,
                        cf.CpfCnpj,
                        cf.Rg,
                        cf.RazaoSocial,
                        cf.NomeFantasia,
                        cf.NaturezaJuridica,
                        cf.OptanteMEI,
                        cf.OptanteSimples,
                        cf.RegimeTributario,
                        cf.InscricaoEstadual,
                        cf.InscricaoMunicipal,
                        cf.Cnae,
                        cf.AtividadeCnae,
                        cf.Site,
                        cf.StatusCadastro,
                        cf.DataInclusao,
                        cf.IdRespInclusao,
                        cf.NomeRespInclusao,
                        cf.IdRespUltimaModificacao,
                        cf.NomeRespUltimaModificacao,
                        cf.DataUltimaModificacao,
                        cf.DataInativacao,
                        cf.IdRespInativacao,
                        cf.NomeRespInativacao,
                        cf.JustificativaInativacao,
                        cf.Observacoes
                    FROM ClientesFornecedores cf
                    WHERE 1=1";

            string countSql = "SELECT COUNT(*) FROM ClientesFornecedores WHERE 1=1";

            if (!string.IsNullOrEmpty(razaoSocial))
            {
                sql += " AND cf.RazaoSocial LIKE @RAZAO_SOCIAL";
                countSql += " AND RazaoSocial LIKE @RAZAO_SOCIAL";
            }

            if (!string.IsNullOrEmpty(nomeFantasia))
            {
                sql += " AND cf.NomeFantasia LIKE @NOME_FANTASIA";
                countSql += " AND NomeFantasia LIKE @NOME_FANTASIA";
            }

            if (!string.IsNullOrEmpty(cpfCnpj))
            {
                sql += " AND cf.CpfCnpj LIKE @CPF_CNPJ";
                countSql += " AND CpfCnpj LIKE @CPF_CNPJ";
            }

            if (tipoCadastro.HasValue)
            {
                sql += " AND cf.TipoCadastro = @TIPO_CADASTRO";
                countSql += " AND TipoCadastro = @TIPO_CADASTRO";
            }

            if (tipoPessoa.HasValue)
            {
                sql += " AND cf.TipoPessoa = @TIPO_PESSOA";
                countSql += " AND TipoPessoa = @TIPO_PESSOA";
            }

            if (statusCadastro.HasValue)
            {
                sql += " AND cf.StatusCadastro = @STATUS_CADASTRO";
                countSql += " AND StatusCadastro = @STATUS_CADASTRO";
            }

            sql += @"
                ORDER BY 
                    cf.Id
                OFFSET 
                    @OFFSET ROWS 
                FETCH NEXT 
                    @PAGE_SIZE ROWS ONLY
                )
        
            SELECT 
                cp.*,
                e.Id as IdEndereco,
                e.IdClienteFornecedor,
                e.Cep,
                e.Logradouro,
                e.TipoEndereco,
                e.Complemento,
                e.Numero,
                e.Cidade,
                e.Bairro,
                e.Uf,
                e.Referencia,
                c.Id as IdContato,
                c.IdClienteFornecedor as ContatoIdClienteFornecedor,
                c.Nome,
                c.Cargo,
                c.Email,
                c.Telefone,
                c.Celular,
                c.Ramal
            FROM ClientesPaginados cp
            LEFT JOIN Enderecos e ON e.IdClienteFornecedor = cp.Id
            LEFT JOIN Contatos c ON c.IdClienteFornecedor = cp.Id";

            using (var connection = _vmiDbContext.Connection)
            {
                var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new
                {
                    RAZAO_SOCIAL = string.IsNullOrEmpty(razaoSocial) ? null : $"%{razaoSocial}%",
                    NOME_FANTASIA = string.IsNullOrEmpty(nomeFantasia) ? null : $"%{nomeFantasia}%",
                    CPF_CNPJ = string.IsNullOrEmpty(cpfCnpj) ? null : $"%{cpfCnpj}%",
                    TIPO_CADASTRO = tipoCadastro,
                    TIPO_PESSOA = tipoPessoa,
                    STATUS_CADASTRO = statusCadastro
                });

                var lookup = new Dictionary<Guid, ClienteFornecedor>();

                await connection.QueryAsync<ClienteFornecedor, Endereco, Contato, ClienteFornecedor>(
                    sql,
                    (cliente, endereco, contato) =>
                    {
                        if (!lookup.TryGetValue(cliente.Id, out var clienteEntry))
                        {
                            clienteEntry = cliente;
                            clienteEntry.Enderecos = new List<Endereco>();
                            clienteEntry.Contatos = new List<Contato>();
                            lookup.Add(clienteEntry.Id, clienteEntry);
                        }

                        if (endereco != null && !clienteEntry.Enderecos.Any(e => e.Id == endereco.Id))
                        {
                            clienteEntry.Enderecos.Add(endereco);
                        }

                        if (contato != null && !clienteEntry.Contatos.Any(c => c.Id == contato.Id))
                        {
                            clienteEntry.Contatos.Add(contato);
                        }

                        return clienteEntry;
                    },
                    new
                    {
                        RAZAO_SOCIAL = string.IsNullOrEmpty(razaoSocial) ? null : $"%{razaoSocial}%",
                        NOME_FANTASIA = string.IsNullOrEmpty(nomeFantasia) ? null : $"%{nomeFantasia}%",
                        CPF_CNPJ = string.IsNullOrEmpty(cpfCnpj) ? null : $"%{cpfCnpj}%",
                        TIPO_CADASTRO = tipoCadastro,
                        TIPO_PESSOA = tipoPessoa,
                        STATUS_CADASTRO = statusCadastro,
                        OFFSET = (pageNumber - 1) * pageSize,
                        PAGE_SIZE = pageSize
                    },
                splitOn: "IdEndereco,IdContato");

                var clientesFornecedores = lookup.Values.ToList();

                return new PagedResult<ClienteFornecedor>
                {
                    Items = clientesFornecedores,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task Remover(Guid id)
        {
            var sqlEnderecos = "DELETE FROM Enderecos WHERE IdClienteFornecedor = @Id;";
            await _vmiDbContext.Connection.ExecuteAsync(sqlEnderecos, new { Id = id });

            string sqlDadosFinanceiros = "DELETE FROM DadosFinanceiros WHERE IdClienteFornecedor = @Id";
            await _vmiDbContext.Connection.ExecuteAsync(sqlDadosFinanceiros, new { Id = id });

            string sqlContatos = "DELETE FROM Contatos WHERE IdClienteFornecedor = @Id";
            await _vmiDbContext.Connection.ExecuteAsync(sqlContatos, new { Id = id });

            var sqlClientesFornecedores = "DELETE FROM ClientesFornecedores WHERE Id = @Id;";
            await _vmiDbContext.Connection.ExecuteAsync(sqlClientesFornecedores, new { Id = id });
        }

        public async Task<ClienteFornecedor> Atualizar(ClienteFornecedor clienteFornecedor)
        {
            try
            {
                var sqlSet = new List<string>();
                var parameters = new DynamicParameters();

                parameters.Add("Id", clienteFornecedor.Id);
                parameters.Add("RazaoSocial", clienteFornecedor.RazaoSocial);
                parameters.Add("NomeFantasia", clienteFornecedor.NomeFantasia);
                parameters.Add("CpfCnpj", clienteFornecedor.CpfCnpj);
                parameters.Add("TipoCadastro", clienteFornecedor.TipoCadastro);
                parameters.Add("TipoPessoa", clienteFornecedor.TipoPessoa);
                parameters.Add("TipoEmpresa", clienteFornecedor.TipoEmpresa);
                parameters.Add("PorteEmpresa", clienteFornecedor.PorteEmpresa);
                parameters.Add("NaturezaJuridica", clienteFornecedor.NaturezaJuridica);
                parameters.Add("OptanteMEI", clienteFornecedor.OptanteMEI);
                parameters.Add("OptanteSimples", clienteFornecedor.OptanteSimples);
                parameters.Add("Rg", clienteFornecedor.Rg);
                parameters.Add("RegimeTributario", clienteFornecedor.RegimeTributario);
                parameters.Add("InscricaoEstadual", clienteFornecedor.InscricaoEstadual);
                parameters.Add("InscricaoMunicipal", clienteFornecedor.InscricaoMunicipal);
                parameters.Add("Cnae", clienteFornecedor.Cnae);
                parameters.Add("AtividadeCnae", clienteFornecedor.AtividadeCnae);
                parameters.Add("Site", clienteFornecedor.Site);
                parameters.Add("Observacoes", clienteFornecedor.Observacoes);
                parameters.Add("IdRespUltimaModificacao", clienteFornecedor.IdRespUltimaModificacao);
                parameters.Add("NomeRespUltimaModificacao", clienteFornecedor.NomeRespUltimaModificacao);
                parameters.Add("DataUltimaModificacao", clienteFornecedor.DataUltimaModificacao);

                sqlSet.Add("RazaoSocial = @RazaoSocial");
                sqlSet.Add("NomeFantasia = @NomeFantasia");
                sqlSet.Add("CpfCnpj = @CpfCnpj");
                sqlSet.Add("TipoCadastro = @TipoCadastro");
                sqlSet.Add("TipoPessoa = @TipoPessoa");
                sqlSet.Add("TipoEmpresa = @TipoEmpresa");
                sqlSet.Add("PorteEmpresa = @PorteEmpresa");
                sqlSet.Add("NaturezaJuridica = @NaturezaJuridica");
                sqlSet.Add("OptanteMEI = @OptanteMEI");
                sqlSet.Add("OptanteSimples = @OptanteSimples");
                sqlSet.Add("Rg = @Rg");
                sqlSet.Add("RegimeTributario = @RegimeTributario");
                sqlSet.Add("InscricaoEstadual = @InscricaoEstadual");
                sqlSet.Add("InscricaoMunicipal = @InscricaoMunicipal");
                sqlSet.Add("Cnae = @Cnae");
                sqlSet.Add("AtividadeCnae = @AtividadeCnae");
                sqlSet.Add("Site = @Site");
                sqlSet.Add("Observacoes = @Observacoes");
                sqlSet.Add("IdRespUltimaModificacao = @IdRespUltimaModificacao");
                sqlSet.Add("NomeRespUltimaModificacao = @NomeRespUltimaModificacao");
                sqlSet.Add("DataUltimaModificacao = @DataUltimaModificacao");

                if (clienteFornecedor.StatusCadastro)
                {
                    sqlSet.Add("StatusCadastro = @StatusCadastro");
                    parameters.Add("StatusCadastro", clienteFornecedor.StatusCadastro);

                    sqlSet.Add("DataInativacao = NULL");
                    sqlSet.Add("IdRespInativacao = NULL");
                    sqlSet.Add("NomeRespInativacao = NULL");
                    sqlSet.Add("JustificativaInativacao = NULL");
                }
                else
                {
                    sqlSet.Add("StatusCadastro = @StatusCadastro");
                    parameters.Add("StatusCadastro", clienteFornecedor.StatusCadastro);

                    if (clienteFornecedor.IdRespInativacao != null)
                    {
                        sqlSet.Add("IdRespInativacao = @IdRespInativacao");
                        parameters.Add("IdRespInativacao", clienteFornecedor.IdRespInativacao);

                        sqlSet.Add("NomeRespInativacao = @NomeRespInativacao");
                        parameters.Add("NomeRespInativacao", clienteFornecedor.NomeRespInativacao);
                    }

                    sqlSet.Add("DataInativacao = @DataInativacao");
                    parameters.Add("DataInativacao", DateTime.Now);

                    if (clienteFornecedor.JustificativaInativacao != null)
                    {
                        sqlSet.Add("JustificativaInativacao = @JustificativaInativacao");
                        parameters.Add("JustificativaInativacao", clienteFornecedor.JustificativaInativacao);
                    }
                }

                var sqlUpdateCliente = $@"
                UPDATE ClientesFornecedores
                SET {string.Join(", ", sqlSet)}
                WHERE Id = @Id";

                await _vmiDbContext.Connection.ExecuteAsync(sqlUpdateCliente, parameters);

                const string sqlDeleteEnderecos = "DELETE FROM Enderecos WHERE IdClienteFornecedor = @Id";
                await _vmiDbContext.Connection.ExecuteAsync(sqlDeleteEnderecos, new { clienteFornecedor.Id });

                const string sqlInsertEndereco = @"
                INSERT INTO Enderecos (
                    Id, 
                    IdClienteFornecedor,
                    Cep, Logradouro,
                    TipoEndereco,
                    Complemento,
                    Numero,
                    Cidade,
                    Bairro,
                    Uf,
                    Referencia
                )
                VALUES (
                    @Id,
                    @IdClienteFornecedor,
                    @Cep,
                    @Logradouro,
                    @TipoEndereco,
                    @Complemento,
                    @Numero,
                    @Cidade,
                    @Bairro,
                    @Uf,
                    @Referencia
                )";

                foreach (var endereco in clienteFornecedor.Enderecos)
                {
                    endereco.IdClienteFornecedor = clienteFornecedor.Id;
                    await _vmiDbContext.Connection.ExecuteAsync(sqlInsertEndereco, endereco);
                }

                const string sqlDeleteContatos = "DELETE FROM Contatos WHERE IdClienteFornecedor = @Id";
                await _vmiDbContext.Connection.ExecuteAsync(sqlDeleteContatos, new { clienteFornecedor.Id });

                const string sqlInsertContato = @"
                INSERT INTO Contatos (
                    Id,
                    IdClienteFornecedor,
                    Nome,
                    Cargo,
                    Email,
                    Telefone,
                    Celular,
                    Ramal
                )
                VALUES (
                    @Id,
                    @IdClienteFornecedor,
                    @Nome,
                    @Cargo,
                    @Email,
                    @Telefone,
                    @Celular,
                    @Ramal
                )";

                foreach (var contato in clienteFornecedor.Contatos)
                {
                    contato.IdClienteFornecedor = clienteFornecedor.Id;
                    await _vmiDbContext.Connection.ExecuteAsync(sqlInsertContato, contato);
                }

                const string sqlDeleteDadosFinanceiros = "DELETE FROM DadosFinanceiros WHERE IdClienteFornecedor = @Id";
                await _vmiDbContext.Connection.ExecuteAsync(sqlDeleteDadosFinanceiros, new { clienteFornecedor.Id });

                const string sqlInsertDadoFinanceiro = @"
                INSERT INTO DadosFinanceiros (
                    Id,
                    IdClienteFornecedor,
                    LimiteCredito,
                    CondicaoPagamento,
                    Banco,
                    Agencia,
                    Conta,
                    TipoConta,
                    ChavePix
                )
                VALUES (
                    @Id,
                    @IdClienteFornecedor,
                    @LimiteCredito,
                    @CondicaoPagamento,
                    @Banco,
                    @Agencia,
                    @Conta,
                    @TipoConta,
                    @ChavePix
                )";

                foreach (var dado in clienteFornecedor.DadosFinanceiros)
                {
                    dado.IdClienteFornecedor = clienteFornecedor.Id;
                    await _vmiDbContext.Connection.ExecuteAsync(sqlInsertDadoFinanceiro, dado);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return clienteFornecedor;
        }

        public async Task<ClienteFornecedor> Adicionar(ClienteFornecedor clienteFornecedor)
        {

            const string sqlInsertCliente = @"
            INSERT INTO ClientesFornecedores (
                Id,
                RazaoSocial,
                NomeFantasia,
                CpfCnpj,
                TipoCadastro,
                TipoPessoa,
                TipoEmpresa,
                PorteEmpresa,
                NaturezaJuridica,
                OptanteMEI,
                OptanteSimples,
                InscricaoEstadual,
                InscricaoMunicipal,
                Cnae,
                AtividadeCnae,
                Site,
                StatusCadastro,
                IdRespInclusao,
                DataInclusao,
                NomeRespInclusao
            )
            VALUES (
                @Id,
                @RazaoSocial, 
                @NomeFantasia, 
                @CpfCnpj,
                @TipoCadastro, 
                @TipoPessoa,
                @TipoEmpresa,
                @PorteEmpresa,
                @NaturezaJuridica,
                @OptanteMEI,
                @OptanteSimples,
                @InscricaoEstadual,
                @InscricaoMunicipal,
                @Cnae,
                @AtividadeCnae,
                @Site,
                @StatusCadastro, 
                @IdRespInclusao, 
                @DataInclusao, 
                @NomeRespInclusao
            )";

            await _vmiDbContext.Connection.ExecuteAsync(sqlInsertCliente, clienteFornecedor);

            const string sqlInsertEndereco = @"
            INSERT INTO Enderecos (
                Id,
                IdClienteFornecedor,
                Cep,
                Logradouro,
                TipoEndereco,
                Complemento,
                Numero,
                Cidade,
                Bairro,
                Uf,
                Referencia
            )
            VALUES (
                @Id, 
                @IdClienteFornecedor,
                @Cep,
                @Logradouro,
                @TipoEndereco,
                @Complemento,
                @Numero,
                @Cidade,
                @Bairro,
                @Uf,
                @Referencia
            )";

            foreach (var endereco in clienteFornecedor.Enderecos)
            {
                endereco.IdClienteFornecedor = clienteFornecedor.Id;
                await _vmiDbContext.Connection.ExecuteAsync(sqlInsertEndereco, endereco);
            }

            const string sqlInsertContato = @"
            INSERT INTO Contatos (
                Id,
                IdClienteFornecedor,
                Nome,
                Cargo,
                Email,
                Telefone,
                Celular,
                Ramal
            )
            VALUES (
                @Id,
                @IdClienteFornecedor,
                @Nome, 
                @Cargo,
                @Email,
                @Telefone,
                @Celular,
                @Ramal
            )";

            foreach (var contato in clienteFornecedor.Contatos)
            {
                contato.IdClienteFornecedor = clienteFornecedor.Id;
                await _vmiDbContext.Connection.ExecuteAsync(sqlInsertContato, contato);
            }

            if(clienteFornecedor.TipoCadastro == 0)
            {
                const string sqlInsertDadoFinanceiro = @"
                INSERT INTO DadosFinanceiros (
                    Id,
                    IdClienteFornecedor,
                    LimiteCredito,
                    CondicaoPagamento,
                    Banco,
                    Agencia,
                    Conta,
                    TipoConta,
                    ChavePix
                )
                VALUES (
                    @Id,
                    @IdClienteFornecedor,
                    @LimiteCredito,
                    @CondicaoPagamento,
                    @Banco,
                    @Agencia,
                    @Conta,
                    @TipoConta,
                    @ChavePix
                )";

                foreach (var dado in clienteFornecedor.DadosFinanceiros)
                {
                    dado.IdClienteFornecedor = clienteFornecedor.Id;
                    await _vmiDbContext.Connection.ExecuteAsync(sqlInsertDadoFinanceiro, dado);
                }
            }

            if (clienteFornecedor.Anexos != null && clienteFornecedor.Anexos.Any())
            {
                const string sqlInsertAnexo = @"
                INSERT INTO Anexos (
                    Id,
                    IdClienteFornecedor,
                    Nome,
                    Conteudo,
                    Extensao,
                    IdRespInclusao,
                    DataInclusao,
                    NomeRespInclusao
                )
                VALUES (
                    @Id,
                    @IdClienteFornecedor,
                    @Nome,
                    @Conteudo,
                    @Extensao,
                    @IdRespInclusao,
                    @DataInclusao,
                    @NomeRespInclusao
                )";

                foreach (var anexo in clienteFornecedor.Anexos)
                {
                    anexo.IdClienteFornecedor = clienteFornecedor.Id;
                    anexo.IdRespInclusao = clienteFornecedor.IdRespInclusao;
                    anexo.NomeRespInclusao = clienteFornecedor.NomeRespInclusao;
                    anexo.DataInclusao = clienteFornecedor.DataInclusao;

                    await _vmiDbContext.Connection.ExecuteAsync(sqlInsertAnexo, anexo);
                }
            }

            return clienteFornecedor;
        }

    }
}