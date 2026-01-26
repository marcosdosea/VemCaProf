using Core.DTO;

namespace Core.Mappers
{
    public static class PessoaMapper
    {
        // MÉTODOS DE IDA (DTO -> ENTITY) - Usados no CREATE/EDIT
        
        // 1. PROFESSOR
        public static Pessoa ToEntity(ProfessorPessoaDTO dto)
        {
            var pessoa = MapBase(dto); 

            pessoa.IdCidade = dto.IdCidade;
            pessoa.Libras = dto.Libras;
            pessoa.DescricaoProfessor = dto.DescricaoProfessor;
            
            pessoa.Diploma = dto.Diploma;
            pessoa.FotoDocumento = dto.FotoDocumento;
            pessoa.FotoPerfil = dto.FotoPerfil;

            return pessoa;
        }

        // 2. ALUNO
        public static Pessoa ToEntity(AlunoPessoaDTO dto)
        {
            var pessoa = MapBase(dto);

            pessoa.AlunoDeMenor = dto.AlunoDeMenor;
            pessoa.Atipico = dto.Atipico;
            
            pessoa.ResponsavelId = dto.IdResponsavel;
            
            
            pessoa.IdCidade = 0; 

            return pessoa;
        }

        // 3. RESPONSÁVEL
        public static Pessoa ToEntity(ResponsavelPessoaDTO dto)
        {
            var pessoa = MapBase(dto);

            // Mapeia o específico de Responsável
            pessoa.QuantidadeDeDependentes = dto.QuantidadeDeDependentes;
            
            pessoa.IdCidade = 0;

            return pessoa;
        }

        // Auxiliar de Ida
        private static Pessoa MapBase(PessoaDTO dto)
        {
            return new Pessoa
            {
                Id = dto.Id, 
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Cpf = dto.Cpf,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Genero = dto.Genero,
                DataNascimento = dto.DataNascimento,

                // Endereço de Moradia (Comum a todos)
                Cep = dto.Cep,
                Rua = dto.Rua,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                
            };
        }
        
        // MÉTODOS DE VOLTA (ENTITY -> DTO) - Usados no GET
        
        public static ProfessorPessoaDTO ToProfessorDTO(Pessoa entity)
        {
            var dto = new ProfessorPessoaDTO();
            PreencherBase(dto, entity);
            
            dto.DescricaoProfessor = entity.DescricaoProfessor;
            dto.Libras = entity.Libras;
            dto.IdCidade = entity.IdCidade;
            dto.Diploma = entity.Diploma;
            dto.FotoDocumento = entity.FotoDocumento;
            dto.FotoPerfil = entity.FotoPerfil;
            
            return dto;
        }

        public static AlunoPessoaDTO ToAlunoDTO(Pessoa entity)
        {
            var dto = new AlunoPessoaDTO();
            PreencherBase(dto, entity);
            
            dto.AlunoDeMenor = entity.AlunoDeMenor;
            dto.Atipico = entity.Atipico;
            
            dto.IdResponsavel = entity.ResponsavelId;
            
            return dto;
        }
        
        // Auxiliar de Volta
        private static void PreencherBase(PessoaDTO dto, Pessoa entity)
        {
            dto.Id = entity.Id;
            dto.Nome = entity.Nome;
            dto.Sobrenome = entity.Sobrenome;
            dto.Email = entity.Email;
            dto.Senha = ""; // Nunca retorna a senha
            dto.Telefone = entity.Telefone;
            dto.Genero = entity.Genero;
            dto.DataNascimento = entity.DataNascimento;
            
            dto.Cep = entity.Cep;
            dto.Rua = entity.Rua;
            dto.Numero = entity.Numero;
            dto.Complemento = entity.Complemento;
            dto.Bairro = entity.Bairro;
            dto.Cidade = entity.Cidade;
            dto.Estado = entity.Estado;
          
            
            
        }
    }
}