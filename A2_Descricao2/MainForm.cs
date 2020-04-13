using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace A2_Descricao2
{
    public partial class MainForm : Form
    {

        private class Celula
        {
            public int linha;
            public int coluna;
            public double g;    // o valor da função g para algoritmos guloso
            public double h;    // o valor da função h para algoritmos guloso
            public double f;    //o valor da função f para algoritmos guloso 
            public int level;   //usado como um segundo parametro para ordenar o conjunto aberto 
            public Celula ant;   // cada estado corresponde a uma celula e cada estado possui um antecessor, que é armazenado nessa variavel
            public Celula(int lin, int col)
            {
                this.linha = lin;
                this.coluna = col;
            }
        }

        const int VAZIO = 0;      // celula vazia
        const int OBSTACULO = 1;       // celula com obstaculo
        const int BOT = 2;      // posicao do bot
        const int OBJETIVO = 3;     // posicao do objetivo
        const int FRONTEIRA = 4;   // celulas que são fronteiras (conjuntoAberto)
        const int FECHADO = 5;     // celulas que são fechadas (conjuntoFechado)
        const int CAMINHO = 6;      // celulas que são caminho do bot para o objetivo

        int linhas;
        int colunas;
        int tamanhoQuadrado;

        List<Celula> conjuntoAberto = new List<Celula>();
        List<Celula> conjuntoFechado = new List<Celula>();

        Celula posicaoBot;  // posicao inicial do bot
        Celula posicaoObjetivo;   // posicao do objetivo

        int[,] grade;      // a grade
        Point[,] centros; // os centros das celulas da grade 
        int expandido;     // numero de nós que foram expandidos
        int level;        // acompanha a criação dos sucessores
        int lin_atual, col_atual, val_atual;
        bool achado;       // objetivo achado
        bool procurando;   // procurando o objetivo
        bool fimDaProcura; // procura encerrada
        bool animacao;   // animacao sendo rodada
        bool mouse_down = false; // click do mouse

        public MainForm()
        {
            InitializeComponent();
            dfs.Checked = true;
            InicializaGrade(true);
        }

        // acha em que celula o mouse está apontando e retorna a celula apontada
        private Celula achaLinhaColuna(int y_atual, int x_atual)
        {
            int lin, col;
            lin = (y_atual - 10) / tamanhoQuadrado;
            col = (x_atual - 10) / tamanhoQuadrado;

            return new Celula(lin, col);
        }

        // lida com os cliques do mouse
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_down = true;
            Celula celula_atual = achaLinhaColuna(e.Y, e.X);
            int lin = celula_atual.linha;
            int col = celula_atual.coluna;

            if (lin >= 0 && lin < linhas && col >= 0 && col < colunas)
            {
                if (!procurando)
                {
                    lin_atual = lin;
                    col_atual = col;
                    val_atual = grade[lin, col];
                }
                else
                {
                    Invalidate();
                }
            }
        }

        //arrastar o objetivo ou o bot
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_down)
            {
                return;
            }

            Celula celula_atual = achaLinhaColuna(e.Y, e.X);
            int lin = celula_atual.linha;
            int col = celula_atual.coluna;

            if (lin >= 0 && lin < linhas && col >= 0 && col < colunas)
            {
                if (!procurando)
                {
                    if (!(lin == lin_atual && col == col_atual) && (val_atual == BOT || val_atual == OBJETIVO))
                    {
                        int novo_val = grade[lin, col];
                        if (novo_val == VAZIO)
                        {
                            grade[lin, col] = val_atual;
                            if (val_atual == BOT)
                            {
                                posicaoBot.linha = lin;
                                posicaoBot.coluna = col;
                            }
                            else
                            {
                                posicaoObjetivo.linha = lin;
                                posicaoObjetivo.coluna = col;
                            }
                            grade[lin_atual, col_atual] = novo_val;
                            lin_atual = lin;
                            col_atual = col;
                            val_atual = grade[lin, col];
                        }
                    }
                }
                Invalidate();
            }
        }

        //ao soltar o mouse
        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down = false;
        }

        //Cria uma nova grade ou um novo labirinto
        private void InicializaGrade(bool criaLabirinto)
        {
            level = 0;
            linhas = 21;
            colunas = 21;
            grade = new int[linhas, colunas];
            centros = new Point[linhas, colunas];
            posicaoBot = new Celula(linhas - 2, 1);
            posicaoObjetivo = new Celula(1, colunas - 2);

            // Cálculo do tamanho e da altura da célula
            tamanhoQuadrado = (int)(500 / (linhas > colunas ? linhas : colunas));

            // calculo das coordenadas dos centros das celulas
            for (int r = 0; r < linhas; r++)
            {
                for (int c = 0; c < colunas; c++)
                {
                    centros[r, c] = new Point(11 + c * tamanhoQuadrado + tamanhoQuadrado / 2, 11 + r * tamanhoQuadrado + tamanhoQuadrado / 2);
                }
            }
            PreencheGrade();
            if (criaLabirinto)
            {
                var labirinto = new Labirinto(linhas / 2, colunas / 2);
                for (int r = 0; r < linhas; r++)
                {
                    for (int c = 0; c < colunas; c++)
                    {
                        if (Regex.IsMatch(labirinto.labirinto_string.Substring(r * colunas + c, 1), "[+-|]"))
                        {
                            grade[r, c] = OBSTACULO;
                        }
                    }
                }
            }
            Invalidate(); // força recriação do labirinto
        }

        // dá valores inicias para as celulas da grade.
        private void PreencheGrade()
        {
            if (procurando || fimDaProcura)
            {
                for (int r = 0; r < linhas; r++)
                {
                    for (int c = 0; c < colunas; c++)
                    {
                        if (grade[r, c] == FRONTEIRA || grade[r, c] == FECHADO || grade[r, c] == CAMINHO)
                        {
                            grade[r, c] = VAZIO;
                        }
                        if (grade[r, c] == BOT)
                        {
                            posicaoBot = new Celula(r, c);
                        }
                        if (grade[r, c] == OBJETIVO)
                        {
                            posicaoObjetivo = new Celula(r, c);
                        }
                    }


                }
            }
            else
            {
                for (int r = 0; r < linhas; r++)
                {
                    for (int c = 0; c < colunas; c++)
                    {
                        grade[r, c] = VAZIO;
                    }

                }
                posicaoBot = new Celula(linhas - 2, 1);
                posicaoObjetivo = new Celula(1, colunas - 2);
            }
            if (guloso.Checked)
            {
                posicaoBot.g = 0;
                posicaoBot.h = 0;
                posicaoBot.f = 0;
            }
            expandido = 0;
            achado = false;
            procurando = false;
            fimDaProcura = false;

            // PRIMEIROS PASSOS DOS ALGORITMOS

            conjuntoAberto.Clear();
            conjuntoAberto.Add(posicaoBot);
            conjuntoFechado.Clear();
            grade[posicaoObjetivo.linha, posicaoObjetivo.coluna] = OBJETIVO;
            grade[posicaoBot.linha, posicaoBot.coluna] = BOT;

        }

        // ativa radiobutons
        private void AtivaRadioButton()
        {
            dfs.Enabled = true; // Busca por profundidade
            guloso.Enabled = true;
        }

        //desativa radiobuttons
        private void DesativaRadioButton()
        {
            dfs.Enabled = false;
            guloso.Enabled = false;
        }

        private void BotaoLabirintoAcao(object sender, EventArgs e)
        {
            animacao = false;
            BotaoAnimacao.Enabled = true;
            AtivaRadioButton();
            InicializaGrade(true);
        }

        private void BotaoLimparAcao(object sender, EventArgs e)
        {
            animacao = false;
            BotaoAnimacao.Enabled = true;
            BotaoAnimacao.ForeColor = Color.Black;
            AtivaRadioButton();
            PreencheGrade();
            Invalidate();
        }

        private void BotaoAnimacaoAcao(object sender, EventArgs e)
        {
            animacao = true;
            procurando = true;
            DesativaRadioButton();
            timer.Stop();
            timer.Interval = 70;
            timer.Start();
            Acao_Animacao();
        }

        //Ação realizada para a Animacao
        private void Acao_Animacao()
        {
            if (animacao)
            {
                ChecarTermino();
                Invalidate();
                if (fimDaProcura)
                {
                    animacao = false;
                    timer.Stop();
                }
            }
        }

        //Ação executada após o intervalo de tempo do Timer
        private void Marca_Timer(object sender, EventArgs e)
        {
            Acao_Animacao();
        }

        // Verifica se continua a procurar ou para
        private void ChecarTermino()
        {

            ExpandirNo();
            if (achado)
            {
                fimDaProcura = true;
                MostrarRota();
                BotaoAnimacao.Enabled = false;
                Invalidate();
            }

        }

        //Expande nós e cria sucessores
        private void ExpandirNo()
        {

            Celula atual;
            if (dfs.Checked)
            {
                atual = (Celula)conjuntoAberto[0];
                conjuntoAberto.RemoveAt(0);
            }
            else
            {   
                // Ordena de acordo com a heuristica para pegar sempre o menor valor de 'f'
                // Se houverem valores iguais para 'f' entao ordena também pelo menor valor de 'level' 
                conjuntoAberto.Sort((x, y) =>
                {
                    int resultado = x.f.CompareTo(y.f);
                    if (resultado == 0)
                        resultado = x.level.CompareTo(y.level);
                    return resultado;
                });
                atual = (Celula)conjuntoAberto[0];
                conjuntoAberto.RemoveAt(0);
            }
            // então adiciona ao conjunto fechado
            conjuntoFechado.Insert(0, atual);
            // atualiza a cor da celula
            grade[atual.linha, atual.coluna] = FECHADO;
            // se o nó for o objetivo
            if (atual.linha == posicaoObjetivo.linha && atual.coluna == posicaoObjetivo.coluna)
            {
                // realiza o processo de finalização
                Celula last = posicaoObjetivo;
                last.ant = atual.ant;
                conjuntoFechado.Add(last);
                achado = true;
                return;
            }
            // Conta os nós que foram expandidos
            expandido++;

            List<Celula> sucessores = CriaSucessores(atual, false);

            foreach (Celula celula in sucessores)
            {
                if (dfs.Checked)
                {
                    //adiciona um sucessor no inicio do conjunto aberto
                    conjuntoAberto.Insert(0, celula);
                    // muda a cor da celula
                    grade[celula.linha, celula.coluna] = FRONTEIRA;
                }
                else if (guloso.Checked)
                {
                    //Heuristica
                    //Calculamos a distancia entre a posição da celula que está sendo avaliada e a posição do objetivo
                    //Esse calculo é armazenado em celula.f 
                    double dxh = centros[posicaoObjetivo.linha, posicaoObjetivo.coluna].X - centros[celula.linha, celula.coluna].X;
                    double dyh = centros[posicaoObjetivo.linha, posicaoObjetivo.coluna].Y - centros[celula.linha, celula.coluna].Y;
                    celula.g = 0;                
                    celula.h = Math.Abs(dxh) + Math.Abs(dyh);
                    celula.f = celula.g + celula.h;

                    int indexConjuntoAberto = EstaNaLista(conjuntoAberto, celula);
                    int indexConjuntoFechado = EstaNaLista(conjuntoFechado, celula);
                    
                    //A celula não está em nenhum dos conjunto, então é adicionada no conjunto aberto
                    if (indexConjuntoAberto == -1 && indexConjuntoFechado == -1)
                    {
                        conjuntoAberto.Add(celula);                        
                        grade[celula.linha, celula.coluna] = FRONTEIRA;                      
                    }
                    else
                    {
                        // A celula está no conjunto aberto
                        if (indexConjuntoAberto > -1)
                        {                           
                            Celula openSetCell = (Celula)conjuntoAberto[indexConjuntoAberto];
                            if (openSetCell.f > celula.f)
                            {                                
                                conjuntoAberto.RemoveAt(indexConjuntoAberto);
                                conjuntoAberto.Add(celula);
                                grade[celula.linha, celula.coluna] = FRONTEIRA;
                            }                           
                        }
                        //A celula está no conjunto fechado
                        else
                        {
                            Celula closedSetCell = (Celula)conjuntoFechado[indexConjuntoFechado];
                            if (closedSetCell.f > celula.f)
                            {                              
                                conjuntoFechado.RemoveAt(indexConjuntoFechado);                              
                                conjuntoAberto.Add(celula);                                
                                grade[celula.linha, celula.coluna] = FRONTEIRA;
                            }
                        }
                    }
                }
            }
        }


        private List<Celula> CriaSucessores(Celula atual, bool conectar)
        {
            int lin = atual.linha;
            int col = atual.coluna;
            // cria uma lista vazia de sucessores para a celula
            List<Celula> temp = new List<Celula>();

            // Regra da heuristica
            // 1: Cima 2: Direita 3: Baixo 4: Esquerda

            // Se não estiver no limite superior do nó
            // e a celula de cima não é um obstáculo
            if (lin > 0 && grade[lin - 1, col] != OBSTACULO && ((dfs.Checked) ?
                          EstaNaLista(conjuntoAberto, new Celula(lin - 1, col)) == -1 &&
                          EstaNaLista(conjuntoFechado, new Celula(lin - 1, col)) == -1 : true))
            {
                Celula celula = new Celula(lin - 1, col);

                // atualiza para celula atual
                celula.ant = atual;
                // e adiciona a celula de cima aos sucessores da atual.
                celula.level = ++level;
                temp.Add(celula);

            }


            // Se não estiver no limite mais à direita do nó
            // e a celula do lado direito não é um obstáculo
            if (col < colunas - 1 && grade[lin, col + 1] != OBSTACULO &&
                    //Se não pertence a nenhum dos conjuntos (aberto ou fechado)
                    ((dfs.Checked) ?
                          EstaNaLista(conjuntoAberto, new Celula(lin, col + 1)) == -1 &&
                          EstaNaLista(conjuntoFechado, new Celula(lin, col + 1)) == -1 : true))
            {
                Celula celula = new Celula(lin, col + 1);

                //atualiza para celula atual
                celula.ant = atual;
                // e adiciona a celula da direita aos sucessores da atual.
                celula.level = ++level;
                temp.Add(celula);

            }

            // Se não estiver no limite inferior do nó
            // e a celula do lado de baixo não é um obstáculo ...
            if (lin < linhas - 1 && grade[lin + 1, col] != OBSTACULO &&
                    //Se não pertence a nenhum dos conjuntos (aberto ou fechado)
                    ((dfs.Checked) ?
                          EstaNaLista(conjuntoAberto, new Celula(lin + 1, col)) == -1 &&
                          EstaNaLista(conjuntoFechado, new Celula(lin + 1, col)) == -1 : true))
            {
                Celula celula = new Celula(lin + 1, col);

                //atualiza para celula atual
                celula.ant = atual;
                // e adiciona a celula de baixo aos sucessores da atual.
                celula.level = ++level;
                temp.Add(celula);

            }


            // Se não estiver no limite mais à esquerda do nó
            // e a celula do lado esquerdo não é um obstáculo ...
            if (col > 0 && grade[lin, col - 1] != OBSTACULO &&
                    //Se não pertence a nenhum dos conjuntos (aberto ou fechado)
                    ((dfs.Checked) ?
                          EstaNaLista(conjuntoAberto, new Celula(lin, col - 1)) == -1 &&
                          EstaNaLista(conjuntoFechado, new Celula(lin, col - 1)) == -1 : true))
            {
                Celula celula = new Celula(lin, col - 1);

                //atualiza para celula atual
                celula.ant = atual;
                // e adiciona a celula da esquerda aos sucessores da atual.
                celula.level = ++level;
                temp.Add(celula);

            }


            // Quando o algoritmo DFS está em uso, as células são adicionadas uma a uma no início do
            // conjuntoAberto. Por isso, devemos reverter a ordem dos sucessores formados,
            // para que o sucessor correspondente à prioridade mais alta seja colocado
            // o primeiro da lista.
            // Para o guloso não são problema, porque a lista está classificada
            // de acordo com 'f' ou 'distancia' antes de extrair o primeiro elemento dele.
            if (dfs.Checked)
            {
                temp.Reverse();
            }
            return temp;
        }

        // Retorna o índice da celula 'atual' na lista 'lista'
        // O índice da celula na lista. Se a celula não for achado retorna -1 
        private int EstaNaLista(List<Celula> lista, Celula atual)
        {
            int index = -1;
            for (int i = 0; i < lista.Count; i++)
            {
                Celula ListarItem = (Celula)lista[i];
                if (atual.linha == ListarItem.linha && atual.coluna == ListarItem.coluna)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void CaixaMensagem(object sender, EventArgs e)
        {

        }

        private void MostrarRota()
        {
            int passos = 0;
            double distancia = 0;
            int index = EstaNaLista(conjuntoFechado, posicaoObjetivo);
            Celula celula = (Celula)conjuntoFechado[index];
            grade[celula.linha, celula.coluna] = OBJETIVO;
            do
            {
                passos++;
                double dx = centros[celula.linha, celula.coluna].X - centros[celula.ant.linha, celula.ant.coluna].X;
                double dy = centros[celula.linha, celula.coluna].Y - centros[celula.ant.linha, celula.ant.coluna].Y;
                distancia += Math.Sqrt(dx * dx + dy * dy);
                celula = celula.ant;
                grade[celula.linha, celula.coluna] = CAMINHO;
            }
            while (!(celula.linha == posicaoBot.linha && celula.coluna == posicaoBot.coluna));
            grade[posicaoBot.linha, posicaoBot.coluna] = BOT;
            String msg;
            msg = String.Format("Nós explorados: {0}\nPassos: {1}\nDistancia: {2:N1}", expandido, passos, distancia);
            mensagem.Text = msg;

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush;
            brush = new SolidBrush(Color.DarkGreen);
            g.FillRectangle(brush, new Rectangle(10, 10, colunas * tamanhoQuadrado + 1, linhas * tamanhoQuadrado + 1));
            brush.Dispose();

            for (int l = 0; l < linhas; l++)
            {
                for (int c = 0; c < colunas; c++)
                {
                    if (grade[l, c] == VAZIO)
                    {
                        brush = new SolidBrush(Color.LightGreen);
                    }
                    else if (grade[l, c] == BOT)
                    {
                        brush = new SolidBrush(Color.Blue);
                    }
                    else if (grade[l, c] == OBJETIVO)
                    {
                        brush = new SolidBrush(Color.Red);
                    }
                    else if (grade[l, c] == OBSTACULO)
                    {
                        brush = new SolidBrush(Color.DarkGreen);
                    }
                    else if (grade[l, c] == FRONTEIRA)
                    {
                        brush = new SolidBrush(Color.DarkBlue);
                    }
                    else if (grade[l, c] == FECHADO)
                    {
                        brush = new SolidBrush(Color.Purple);
                    }
                    else if (grade[l, c] == CAMINHO)
                    {
                        brush = new SolidBrush(Color.CornflowerBlue);
                    }
                    g.FillPolygon(brush, CalculaQuadrado(l, c));
                    brush.Dispose();
                }

            }

        }

        // Calcula as coordenadas dos vértices do quadrado representando uma celula
        // retorna as coordenadas dos vértices como uma matriz de pontos
        private Point[] CalculaQuadrado(int l, int c)
        {
            Point[] poligono = {
                new Point((int)(11 +     c*tamanhoQuadrado + 0), (int)(11 +     l*tamanhoQuadrado + 0)),
                new Point((int)(11 + (c+1)*tamanhoQuadrado - 1), (int)(11 +     l*tamanhoQuadrado + 0)),
                new Point((int)(11 + (c+1)*tamanhoQuadrado - 1), (int)(11 + (l+1)*tamanhoQuadrado - 1)),
                new Point((int)(11 +     c*tamanhoQuadrado + 0), (int)(11 + (l+1)*tamanhoQuadrado - 1))
            };
            return poligono;
        }

    }

    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var e = source.ToArray();
            for (var i = e.Length - 1; i >= 0; i--)
            {
                var trocaIndex = rng.Next(i + 1);
                yield return e[trocaIndex];
                e[trocaIndex] = e[i];
            }
        }

        public static EstadoCelula ParedeOposta(this EstadoCelula orig)
        {
            return (EstadoCelula)(((int)orig >> 2) | ((int)orig << 2)) & EstadoCelula.Inicio;
        }

    }

    [Flags]
    public enum EstadoCelula
    {
        Cima = 1,
        Direita = 2,
        Baixo = 4,
        Esquerda = 8,
        Visitado = 128,
        Inicio = Cima | Direita | Baixo | Esquerda,
    }

    public struct RemoveParedeAcao
    {
        public Point Vizinho;
        public EstadoCelula Parede;
    }

    public class Labirinto
    {
        private readonly EstadoCelula[,] _celulas;
        private readonly int _largura;
        private readonly int _altura;
        private readonly Random _rng;
        public String labirinto_string = "";

        public Labirinto(int largura, int altura)
        {
            _largura = largura;
            _altura = altura;
            _celulas = new EstadoCelula[largura, altura];
            for (var x = 0; x < largura; x++)
            {
                for (var y = 0; y < altura; y++)
                {
                    _celulas[x, y] = EstadoCelula.Inicio;
                }
            }
            _rng = new Random();
            VisitarCelula(_rng.Next(largura), _rng.Next(altura));
            FazerString();
        }

        public EstadoCelula this[int x, int y]
        {
            get { return _celulas[x, y]; }
            set { _celulas[x, y] = value; }
        }

        public IEnumerable<RemoveParedeAcao> PegarVizinhos(Point p)
        {
            if (p.X > 0) yield return new RemoveParedeAcao { Vizinho = new Point(p.X - 1, p.Y), Parede = EstadoCelula.Esquerda };
            if (p.Y > 0) yield return new RemoveParedeAcao { Vizinho = new Point(p.X, p.Y - 1), Parede = EstadoCelula.Cima };
            if (p.X < _largura - 1) yield return new RemoveParedeAcao { Vizinho = new Point(p.X + 1, p.Y), Parede = EstadoCelula.Direita };
            if (p.Y < _altura - 1) yield return new RemoveParedeAcao { Vizinho = new Point(p.X, p.Y + 1), Parede = EstadoCelula.Baixo };
        }

        public void VisitarCelula(int x, int y)
        {
            this[x, y] |= EstadoCelula.Visitado;
            foreach (var p in PegarVizinhos(new Point(x, y)).Shuffle(_rng).Where(z => !(this[z.Vizinho.X, z.Vizinho.Y].HasFlag(EstadoCelula.Visitado))))
            {
                this[x, y] -= p.Parede;
                this[p.Vizinho.X, p.Vizinho.Y] -= p.Parede.ParedeOposta();
                VisitarCelula(p.Vizinho.X, p.Vizinho.Y);
            }
        }

        public void FazerString()
        {
            var primeiroNaLinha = string.Empty;
            for (var y = 0; y < _altura; y++)
            {
                var sbCima = new StringBuilder();
                var sbMeio = new StringBuilder();
                for (var x = 0; x < _largura; x++)
                {
                    sbCima.Append(this[x, y].HasFlag(EstadoCelula.Cima) ? "+-" : "+ ");
                    sbMeio.Append(this[x, y].HasFlag(EstadoCelula.Esquerda) ? "| " : "  ");
                }
                if (primeiroNaLinha == string.Empty)
                    primeiroNaLinha = sbCima.ToString();
                labirinto_string = labirinto_string + sbCima + "+";
                labirinto_string = labirinto_string + sbMeio + "|";
            }
            labirinto_string = labirinto_string + primeiroNaLinha + "+";
        }
    }
}
