import { useNavigate } from 'react-router-dom';
import video from './assets/FuriaFan.mp4';
import logo from './Images/Furialogo.png';
import { FaArrowDown } from "react-icons/fa";
import { useState, useRef } from 'react';

function FormularioFuria() {

  const formularioRef = useRef(null);



  const scrollToForm = () => {
    formularioRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState({});
  const [nome, setNome] = useState('');
  const [tempoFuria, setTempoFuria] = useState('menos de 1 ano');
  const [jogoFavorito, setJogoFavorito] = useState('Valorant');
  const [plataforma, setPlataforma] = useState('Pc');
  const [mensagem, setMensagem] = useState('');
  const navigate = useNavigate();

  
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const dados = { nome, tempoFuria, jogoFavorito, plataforma, mensagem };

    const newErrors = {};
    if (!nome.trim()) newErrors.nome = "Nome é obrigatório.";


    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setErrors({}); // limpa erros se tudo ok

    setIsSubmitting(true);

    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Fan`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dados)
      });




      const resultado = await response.json();
      console.log(resultado); // aqui pode exibir feedback ou navegação




      if (!response.ok) {
        const errorData = await response.json();
        console.error('Erro na resposta da API:', errorData);
        alert(`Erro ao enviar! ${response.status}: ${JSON.stringify(errorData)}`);
        setIsSubmitting(false);
        return;
      }


      // Navega para a nova página e passa o resultado como state
      navigate('/result', { state: dados });


    } catch (err) {
      console.log('API URL:', import.meta.env.VITE_API_URL);
      alert(`Erro ao enviar! ${err}`);
      setIsSubmitting(false);
    }
  };

  return (
    <div className='Home'>

      <div className='video-container' style={{ position: 'relative', height: '100vh', overflow: 'hidden' }}>
        <video className='videoF'
          src={video}
          autoPlay
          loop
          muted
          playsInline
        />

        <div className='logoimg'>
          <img src={logo} alt="" />

          <h1>Comece a acompanhar a Furia do seu jeito <br /> Um jeito simples e rápido para estar acompanhando <br /> seu time favorito</h1>

          <div onClick={scrollToForm} className='boxArrow'><FaArrowDown /></div>
        </div>

      </div>

      <h2 className='subH2'>Preecha o formulário para receber conteúdos da Furia </h2>
      <div className='formPart'>
        <form ref={formularioRef} className='form' onSubmit={handleSubmit}>
          <p>Nome</p>
          <input placeholder="Seu Nome" value={nome} onChange={e => setNome(e.target.value)} />
          {errors.nome && <span style={{ color: 'red', fontSize: '13pt', fontFamily: 'Arial, Helvetica, sans-serif' }}>{errors.nome}</span>}
          <p>Tempo de Furia</p>
          <select value={tempoFuria} onChange={e => setTempoFuria(e.target.value)}>
            <option value="menos de 1 ano">Menos de 1 ano</option>
            <option value="1 a 3 anos">1 a 3 anos</option>
            <option value="mais de 3 anos">Mais de 3 anos</option>
          </select>

          <p>Jogo Favorito</p>
          <select value={jogoFavorito} onChange={e => setJogoFavorito(e.target.value)}>
            <option value="Valorant">Valorant</option>
            <option value="Counter Strike 2">Counter Strike 2</option>
            <option value="Rocket League">Rocket League</option>
            <option value="League Of Legends">League Of Legends</option>
            <option value="Rainbow Six">Rainbow Six</option>
            <option value="Apex Legends">Apex Legends</option>
          </select>

          <p>Plataforma</p>
          <select value={plataforma} onChange={e => setPlataforma(e.target.value)}>
            <option value="Pc">Pc</option>
            <option value="Console">Console</option>
            <option value="Celular">Celular</option>
          </select>
          <p>Espaço para pergunta</p>
          <textarea placeholder="Ex: Qual o time de Rocket League da Furia" value={mensagem} onChange={e => setMensagem(e.target.value)} />
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? (
              <div className="spinner" />
            ) : (
              "Enviar"
            )}
          </button>
        </form>
      </div>

      <div className='space'>

      </div>
    </div>
  );
}

export default FormularioFuria;