import React, { useState, useEffect } from "react";
import { useLocation } from 'react-router-dom';

function PaginaResposta() {
    const location = useLocation();
    const resposta = location.state;
    const [recommendations, setRecommendations] = useState([]);
    const [message, setMessage] = useState("");
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
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        jogoFavorito: resposta.jogoFavorito,
                        mensagem: resposta.mensagem,
                    }),
                });

                if (!response.ok) {
                    throw new Error(`Erro ao buscar recomendações: ${response.status}`);
                }

                const data = await response.json();
                setMessage(data.message);
                setRecommendations(data.recommendations || []);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchRecommendations();
    }, [resposta]);

    const getNivelMensagem = (tempo) => {
        switch (tempo) {
            case "menos de 1 ano":
                return "Fã iniciante!";
            case "1 a 3 anos":
                return "Fã Raiz!";
            case "mais de 3 anos":
                return "FURIOSO MASTER!";
            default:
                return "Fã Iniciante!";
        }
    };

    if (error) {
        return <div className="erro">Erro: {error}</div>;
    }

    return (
        <div className="resultPage">
            <div className="afterResult">
                <div className="pageUP">
                    <div className="infoRes">
                        <p>Nome: <span className="spanRes">{resposta.nome}</span></p>
                        <p>Plataforma: <span className="spanRes">{resposta.plataforma}</span></p>
                        <p>Tempo de Furia: <span className="spanRes">{resposta.tempoFuria}</span></p>
                    </div>
                    <div>
                        <h2>Olá <span className="nameh2Name">{resposta.nome}</span>!</h2>
                        <h2>Você é um <span className="nameh2Mensage">{getNivelMensagem(resposta?.tempoFuria)}</span> Veja os conteúdos abaixo da FURIA sobre seus interesses.</h2>
                    </div>
                </div>
                <div className="recommedation">
                    {loading ? (
                        <div>Carregando recomendações...</div>
                    ) : (
                        <div className="recommedationInside">
                            {message && (
                                <div className="mensagem-personalizada">
                                    <p className="message"> {message}</p>
                                </div>
                            )}
                            <h3>Conteúdo Recomendado:</h3>
                            {recommendations.length > 0 ? (
                                <ul>
                                    {recommendations.map((item, index) => (
                                        <li key={index}>
                                            <strong>
                                                {item.type === "YouTube"
                                                    ? "📺"
                                                    : item.type === "Instagram"
                                                    ? "📷"
                                                    : "🔗"}{" "}
                                                {item.type}:
                                            </strong>{" "}
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
            </div>
        </div>
    );
}

export default PaginaResposta;
