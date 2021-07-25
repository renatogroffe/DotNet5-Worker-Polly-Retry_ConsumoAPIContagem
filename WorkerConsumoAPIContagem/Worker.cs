using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerConsumoAPIContagem.Models;
using Polly.Retry;

namespace WorkerConsumoAPIContagem
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly AsyncRetryPolicy _retryPolicy;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration,
            AsyncRetryPolicy retryPolicy)
        {
            _logger = logger;
            _configuration = configuration;
            _retryPolicy  = retryPolicy;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = new HttpClient();
            var urlApiContagem = _configuration["UrlApiContagem"];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var resultado = await _retryPolicy.ExecuteAsync<ResultadoContador>(() =>
                    {
                        return httpClient
                            .GetFromJsonAsync<ResultadoContador>(urlApiContagem);
                    });

                    _logger.LogInformation($"* {DateTime.Now:HH:mm:ss} * " +
                        $"Contador = {resultado.ValorAtual} | " +
                        $"Mensagem = {resultado.MensagemVariavel}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"# {DateTime.Now:HH:mm:ss} # "+
                        $"Falha ao invocar a API: {ex.GetType().FullName} | {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}