// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// site.js - Funcionalidades para VemCaProfWeb

$(document).ready(function () {
    // Cache de seletores para evitar repetição
    const $cpf = $('#Cpf');
    const $telefone = $('#Telefone');
    const $cep = $('#Cep');
    const $rua = $('#Rua');
    const $bairro = $('#Bairro');
    const $cidade = $('#Cidade');
    const $estado = $('#Estado');
    const $deleteId = $('#deleteId');
    const $confirmDeleteModal = $('#confirmDeleteModal');

    // ===== MÁSCARAS =====
    //$cpf.mask('000.000.000-00', { reverse: true });
    //$telefone.mask('(00) 00000-0000');
    //$cep.mask('00000-000');

    // ===== CONSULTA DE CEP VIA VIA CEP =====
    $cep.on('blur', function () {
        let cep = $(this).val().replace(/\D/g, '');

        if (cep.length === 8) {
            $rua.val('Carregando...');
            $bairro.val('Carregando...');
            $cidade.val('Carregando...');
            $estado.val('Carregando...');

            $.getJSON(`https://viacep.com.br/ws/${cep}/json/`)
                .done(function (dados) {
                    // Verifica se a requisição foi bem-sucedida e não há erro
                    if (!dados.erro) {
                        $rua.val(dados.logradouro);
                        $bairro.val(dados.bairro);
                        $cidade.val(dados.localidade);
                        $estado.val(dados.uf);
                    } else {
                        limparCamposEndereco();
                        alert('CEP não encontrado.');
                    }
                })
                .fail(function () {
                    limparCamposEndereco();
                    alert('Erro ao consultar o CEP. Tente novamente.');
                });
        } else if (cep.length > 0) {
            limparCamposEndereco();
            alert('CEP deve conter 8 dígitos.');
        } else {
            limparCamposEndereco();
        }
    });

    function limparCamposEndereco() {
        $rua.val('');
        $bairro.val('');
        $cidade.val('');
        $estado.val('');
    }

    // ===== MODAL DE CONFIRMAÇÃO DE EXCLUSÃO =====
    $confirmDeleteModal.on('show.bs.modal', function (event) {
        let button = $(event.relatedTarget);
        let id = button.data('id');
        $deleteId.val(id);
    });
});