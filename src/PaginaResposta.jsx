import React, { useState, useEffect } from "react";
import { useLocation } from 'react-router-dom';

function PaginaResposta() {
    const location = useLocation();
    const resposta = location.state;
    const [recommendations, setRecommendations] = useState([]);
    const [message, setMessage] = useState(""); // <-- NOVO estado para mensagem
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchRecommendations = async () => {
            if (!resposta) {
                setError("Nenhuma resposta recebida.");
                setLoading(false);
                return;
            }

            try {
                const response = await fetch(`${import.meta.env.VITE_API_URL}/api/ai/recomendar`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        jogoFavorito: resposta.jogoFavorito,
                        mensagem: resposta.mensagem,
                    }),
                });

                if (!response.ok) {
                    throw new Error(`Erro ao buscar recomendações: ${response.status}`);
                }

                const data = await response.json();
                setMessage(data.message); // <-- Armazena a mensagem do backend
                setRecommendations(data.recommendations || []); // <-- Armazena recomendações
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchRecommendations();
    }, [resposta]);

    let nivelExibicao = resposta?.tempoFuria;
    if (nivelExibicao === "menos de 1 ano") {
        nivelExibicao = "Fã iniciante! Veja os conteúdos abaixo da Furia sobre seus interesses.";
    } else if (nivelExibicao === "1 a 3 anos") {
        nivelExibicao = "Fã Raiz! Veja os conteúdos abaixo da Furia sobre seus interesses.";
    } else if (nivelExibicao === "mais de 3 anos") {
        nivelExibicao = "FURIOSO MASTER! Veja os conteúdos abaixo da Furia sobre seus interesses.";
    } else {
        nivelExibicao = "Fã Iniciante! Veja os conteúdos abaixo da Furia sobre seus interesses.";
    }

    if (error) {
        return <div>Erro: {error}</div>;
    }

    return (
        <div className="resultPage">
            <h2>{nivelExibicao}</h2>

            {loading ? (
                <div>Carregando recomendações...</div>
            ) : (
                <div>
                    {message && (
                        <div className="mensagem-personalizada">
                            <p><strong>Mensagem para você:</strong> {message}</p>
                        </div>
                    )}
                    <h3>Conteúdo Recomendado:</h3>
                    {recommendations.length > 0 ? (
                        <ul>
                            {recommendations.map((item, index) => (
                                <li key={index}>
                                    <strong>{item.type}: </strong>
                                    <a href={item.link} target="_blank" rel="noopener noreferrer">
                                        {item.title}
                                    </a>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p>Nenhum conteúdo recomendado encontrado.</p>
                    )}
                </div>
            )}
        </div>
    );
}

export default PaginaResposta;
