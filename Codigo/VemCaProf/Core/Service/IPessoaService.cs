using Core.DTO;

namespace Core.Service;
// Para o output de dados de pessoas (professores, alunos, responsáveis), seria melhor criar os DTOS específicos para cada tipo de pessoa,
// mas para simplificar, estamos retornando a entidade Pessoa diretamente. Em um cenário real, seria ideal mapear para DTOS apropriados.
public interface IPessoaService
{
    // --- PROFESSOR ---
    void CreateProfessor(ProfessorPessoaDTO dto); 
    void EditProfessor(ProfessorPessoaDTO dto);

    Pessoa GetProfessor(int id); 
    IEnumerable<Pessoa> GetAllProfessores();
    IEnumerable<Pessoa> GetProfessoresByNome(string nome);

    // --- ALUNO ---
    void CreateAluno(AlunoPessoaDTO dto);
    void EditAluno(AlunoPessoaDTO dto);
    Pessoa GetAluno(int id);
    IEnumerable<Pessoa> GetAllAlunos();
    IEnumerable<Pessoa> GetAlunosByNome(string nome);

    // --- RESPONSÁVEL ---
    void CreateResponsavel(ResponsavelPessoaDTO dto);
    void EditResponsavel(ResponsavelPessoaDTO dto);
    Pessoa GetResponsavel(int id);
    IEnumerable<Pessoa> GetAllResponsaveis();
    IEnumerable<Pessoa> GetResponsaveisByNome(string nome);


    void Delete(int id); 
}