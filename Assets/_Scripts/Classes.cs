using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.VisualBasic;

[Serializable]
public class FSRU
{
    public string nome;
    public string terminalGnl;
    public string categoria;
    public Coordenadas coordenadas;
    public List<Custo> custos;
    public double gnlFornecido;
    public string custoUsado;

    public FSRU()
    {
        coordenadas = new Coordenadas();
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);


    }

    public void SetCoordenadas(double _lon, double _lat)
    {
        coordenadas = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }

    public void SetCusto(string _tipo, double _valor, string _moeda)
    {
        bool hasCusto = false;
        Custo custo = new Custo
        {
            tipo = _tipo,
            valor = _valor,
            moeda = _moeda
        };
        foreach (Custo c in custos)
        {
            if (c.tipo == custo.tipo)
            {
                c.valor = custo.valor;
                hasCusto = true;
            }
        }

        if (!hasCusto)
            custos.Add(custo);
    }

}

[Serializable]
public class FSU
{
    public string nome;
    public string terminalGnl;
    public string terminalRegasDestino;
    public string terminalCargaDestino;
    public string categoria;
    public Coordenadas coordenadas;
    public List<Custo> custos;
    public double gnlFornecido;
    public string custoUsado;

    public FSU()
    {
        coordenadas = new Coordenadas();
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public FSU(string _nome, string _categoria)
    {
        nome = _nome;
        categoria = _categoria;
        coordenadas = new Coordenadas();
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);

    }

    public void SetCoordenadas(double _lon, double _lat)
    {
        coordenadas = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }

    public void SetCusto(string _tipo, double _valor, string _moeda)
    {
        bool hasCusto = false;
        Custo custo = new Custo
        {
            tipo = _tipo,
            valor = _valor,
            moeda = _moeda
        };
        foreach (Custo c in custos)
        {
            if (c.tipo == custo.tipo)
            {
                c.valor = custo.valor;
                hasCusto = true;
            }
        }

        if (!hasCusto)
            custos.Add(custo);
    }


}

[Serializable]
public class TerminalGNL
{
    public string nome;
    public string categoria;
    public List<Custo> custos;
    public Coordenadas coordenadas;

    public TerminalGNL(string _nome, string _categoria, double _lon, double _lat)
    {
        nome = _nome;
        categoria = _categoria;
        coordenadas = new Coordenadas()
        {
            lon = _lon,
            lat = _lat
        };
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public void AddCusto(string _tipo, double _valor, string _moeda)
    {
        Custo custo = new Custo();
        custo.tipo = _tipo;
        custo.valor = _valor;
        custo.moeda = _moeda;
        custos.Add(custo);
    }

    public void SetCusto(string _tipo, double _valor, string _moeda)
    {
        bool hasCusto = false;
        Custo custo = new Custo
        {
            tipo = _tipo,
            valor = _valor,
            moeda = _moeda
        };
        foreach (Custo c in custos)
        {
            if (c.tipo == custo.tipo)
            {
                c.valor = custo.valor;
                hasCusto = true;
            }
        }

        if (!hasCusto)
            custos.Add(custo);
    }

}

[Serializable]
public class TerminalRegas
{
    public string nome;
    public string categoria;
    public string usina;
    public string terminalGnl;
    public List<Custo> custos;
    public Coordenadas coordenadas;
    public Coordenadas coordenadasMarker;
    public string custoUsado;

    public TerminalRegas()
    {
        coordenadas = new Coordenadas();

        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);


    }

    public TerminalRegas(string _nome)
    {
        nome = _nome;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
        coordenadas = new Coordenadas();

    }

    public void SetTerminal(string _nome, List<Custo> _custos = null)
    {
        nome = _nome;
        if (_custos != null)
            custos = _custos;
    }

    public void SetCoordenadas(double _lon, double _lat)
    {
        coordenadas = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }

    public void SetMarkerCoords(double _lon, double _lat)
    {
        coordenadasMarker = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }

    public void SetCusto(string _tipo, double _valor, string _moeda)
    {
        bool hasCusto = false;
        Custo custo = new Custo
        {
            tipo = _tipo,
            valor = _valor,
            moeda = _moeda
        };
        foreach (Custo c in custos)
        {
            if (c.tipo == custo.tipo)
            {
                c.valor = custo.valor;
                hasCusto = true;
            }
        }

        if (!hasCusto)
            custos.Add(custo);
    }


}

[Serializable]
public class Frota
{
    public string nome;
    public string terminalRegasDestino;
    public string terminalGnlOrigem;
    public Coordenadas coordenadasMarker;
    public double distancia;
    public List<Custo> custos;

    public double fretePorKm;
    public double capacidadeLiquido;
    public double capacidadeEquivGasoso;
    public double recargasDia;


    public Frota(string _nome)
    {
        nome = _nome;


        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public Frota(string _nome, double _distancia)
    {
        nome = _nome;

        distancia = _distancia;

        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public Frota(string _nome, double _distancia, double _fretePorKm)
    {
        nome = _nome;
        distancia = _distancia;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
        fretePorKm = _fretePorKm;
    }

    public Frota(string _nome, double _distancia, double _fretePorKm, double _capacidadeLiq, GasNatural _gas)
    {
        nome = _nome;
        distancia = _distancia;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
        fretePorKm = _fretePorKm;
        capacidadeLiquido = _capacidadeLiq;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;
    }

    public Frota(string _nome, double _distancia, double _fretePorKm, double _capacidadeLiq, GasNatural _gas, Premissas _premissas, Usina _usina)
    {
        nome = _nome;
        distancia = _distancia;
        fretePorKm = _fretePorKm;
        capacidadeLiquido = _capacidadeLiq;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;

        custos = new List<Custo>();
        Custo opex;
        if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
        {
            recargasDia = 0;

            opex = new Custo()
            {
                tipo = "opex",
                valor = 0,
                moeda = "dolar"
            };
        }
        else
        {
            recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;

            opex = new Custo()
            {
                tipo = "opex",
                valor = (2 * distancia) * recargasDia * (365 * _usina.despacho * 0.01f) * fretePorKm / _premissas.cotacaoDolar,
                moeda = "dolar"
            };
        }

        custos.Add(opex);
    }

    public void Set(double _distancia)
    {
        distancia = _distancia;
    }

    public void Set(double _distancia, double _fretePorKm)
    {
        distancia = _distancia;
        fretePorKm = _fretePorKm;
    }

    public void Set(double _distancia, double _fretePorKm, double _capacidadeLiq, GasNatural _gas)
    {
        distancia = _distancia;
        fretePorKm = _fretePorKm;
        capacidadeLiquido = _capacidadeLiq;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;
    }


    public void Set(double _distancia, double _fretePorKm, GasNatural _gas, Usina _usina)
    {
        distancia = _distancia;
        fretePorKm = _fretePorKm;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;
        if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
        {
            recargasDia = 0;
        }
        else
        {
            recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;
        }
    }

    public void Set(double _distancia, double _fretePorKm, double _capacidadeLiquido, GasNatural _gas, Premissas _premissas, Usina _usina)
    {
        distancia = _distancia;
        fretePorKm = _fretePorKm;
        capacidadeLiquido = _capacidadeLiquido;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;

        if (_usina.potencia > 0)
        {
            Custo opex;
            if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
            {
                recargasDia = 0;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = 0,
                    moeda = "dolar"
                };
            }
            else
            {
                recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = (2 * distancia) * recargasDia * (365 * _usina.despacho * 0.01f) * fretePorKm / _premissas.cotacaoDolar,
                    moeda = "dolar"
                };
            }

            bool hasCusto = false;
            foreach (Custo c in custos)
            {
                if (!string.IsNullOrEmpty(c.tipo))
                {
                    if (c.tipo == opex.tipo)
                    {
                        c.valor = opex.valor;
                        hasCusto = true;
                    }
                }
            }

            if (!hasCusto)
                custos.Add(opex);
        }
    }

    public void Set(double _fretePorKm, double _capacidadeLiquido, GasNatural _gas, Premissas _premissas, Usina _usina)
    {
        fretePorKm = _fretePorKm;
        capacidadeLiquido = _capacidadeLiquido;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;
        Custo opex;
        if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
        {
            recargasDia = 0;

            opex = new Custo()
            {
                tipo = "opex",
                valor = 0,
                moeda = "dolar"
            };
        }
        else
        {
            recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;

            opex = new Custo()
            {
                tipo = "opex",
                valor = (2 * distancia) * recargasDia * (365 * _usina.despacho * 0.01f) * fretePorKm / _premissas.cotacaoDolar,
                moeda = "dolar"
            };
        }

        custos.Add(opex);

    }

    public void Set(double _capacidadeLiquido, GasNatural _gas, Premissas _premissas, Usina _usina)
    {
        capacidadeLiquido = _capacidadeLiquido;
        capacidadeEquivGasoso = capacidadeLiquido * _gas.taxaCompressao;

        if (_usina.potencia > 0)
        {
            Custo opex;
            if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
            {
                recargasDia = 0;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = 0,
                    moeda = "dolar"
                };
            }
            else
            {
                recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = (2 * distancia) * recargasDia * (365 * _usina.despacho * 0.01f) * fretePorKm / _premissas.cotacaoDolar,
                    moeda = "dolar"
                };
            }

            foreach (Custo c in custos)
            {
                if (c.tipo.Contains("opex"))
                {
                    custos.Remove(c);
                }
            }
            custos.Add(opex);
        }
    }

    public void Recalculate(Premissas _premissas, Usina _usina)
    {

        if (_usina.potencia > 0)
        {
            Custo opex;
            if (_usina.demandaDiaM3 <= 0 || _usina.despacho <= 0)
            {
                recargasDia = 0;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = 0,
                    moeda = "dolar"
                };
            }
            else if (_usina.demandaDiaM3 > 0)
            {
                recargasDia = _usina.demandaDiaM3 / capacidadeEquivGasoso;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = (2 * distancia) * recargasDia * (365 * _usina.despacho * 0.01f) * fretePorKm / _premissas.cotacaoDolar,
                    moeda = "dolar"
                };
            }
            else
            {
                recargasDia = 0;

                opex = new Custo()
                {
                    tipo = "opex",
                    valor = 0,
                    moeda = "dolar"
                };
            }

            bool hasCusto = false;
            foreach (Custo c in custos)
            {
                if (!string.IsNullOrEmpty(c.tipo))
                {
                    if (c.tipo == opex.tipo)
                    {
                        c.valor = opex.valor;
                        hasCusto = true;
                    }
                }
            }

            if (!hasCusto)
                custos.Add(opex);
        }
    }

    public void SetCapex(double _valor, string _moeda)
    {
        Custo capex = new Custo();
        capex.tipo = "capex";
        capex.valor = _valor;
        capex.moeda = _moeda;

        foreach (Custo c in custos)
            if (c.tipo == "capex")
                custos.Remove(c);

        custos.Add(capex);
    }



}

[Serializable]
public class Usina
{
    public string nome;
    public string malha;

    public double valorMalha;
    public string pontoDeEntrega;

    public string categoria;
    public double potencia;
    public double eficiencia;
    public double heatRate;
    public double demandaDiaPcs;
    public double demandaDiaPci;
    public double demandaDiaM3;
    public double despacho;
    public double demandaAnual;
    public string tipoDeGas;
    public string fonteDoGas;
    public Coordenadas coordenadas;

    public Usina()
    {
        coordenadas = new Coordenadas();
        potencia = 0;
        eficiencia = 0;
        despacho = 0;
        heatRate = 0;
        demandaDiaPcs = 0;
        demandaDiaPci = 0;
        demandaDiaM3 = 0;
        demandaAnual = 0;
    }

    public Usina(string _nome)
    {
        coordenadas = new Coordenadas();

        nome = _nome;
        potencia = 0;
        valorMalha = 0;
        eficiencia = 0;
        despacho = 0;
        heatRate = 0;
        demandaDiaPcs = 0;
        demandaDiaPci = 0;
        demandaDiaM3 = 0;
        demandaAnual = 0;
    }

    public Usina(string _nome, double _potencia, double _eficiencia, GasNatural _gas)
    {
        coordenadas = new Coordenadas();
        nome = _nome;
        valorMalha = 0;
        potencia = _potencia;
        eficiencia = _eficiencia;

        heatRate = 3412 / (eficiencia / 100);
        demandaDiaPcs = potencia * 24 * heatRate / (1000000);
        demandaDiaPci = demandaDiaPcs / _gas.rel_pciPcs;
        demandaDiaM3 = demandaDiaPci * _gas.m3BtuBasePci;
    }

    public Usina(string _nome, double _potencia, double _eficiencia, GasNatural _gas, Premissas _premissas, double _despacho)
    {
        coordenadas = new Coordenadas();
        valorMalha = 0;
        nome = _nome;
        potencia = _potencia;
        eficiencia = _eficiencia;
        despacho = _despacho;
        heatRate = _premissas.fatorBtuKwh / (eficiencia / 100);
        demandaDiaPcs = potencia * 24 * heatRate / 1000000;
        demandaDiaPci = demandaDiaPcs / _gas.rel_pciPcs;
        demandaDiaM3 = demandaDiaPci * _gas.m3BtuBasePci;
        demandaAnual = demandaDiaPci * 365 * despacho * 0.01f;
    }

    public void SetDemandas(double _potencia, double _eficiencia, GasNatural _gas, Premissas _premissas, double _despacho)
    {
        potencia = _potencia;
        eficiencia = _eficiencia;
        despacho = _despacho;
        heatRate = _premissas.fatorBtuKwh / (eficiencia / 100);
        demandaDiaPcs = potencia * 24 * heatRate / 1000000;
        demandaDiaPci = demandaDiaPcs / _gas.rel_pciPcs;
        demandaDiaM3 = demandaDiaPci * _gas.m3BtuBasePci;
        demandaAnual = demandaDiaPci * 365 * despacho * 0.01f;
    }

    public void SetDemandas(GasNatural _gas)
    {
        demandaDiaPcs = potencia * 24 * heatRate / 1000000;
        demandaDiaPci = demandaDiaPcs / _gas.rel_pciPcs;
        demandaDiaM3 = demandaDiaPci * _gas.m3BtuBasePci;
    }

    public void SetCoordenadas(double _lon, double _lat)
    {
        coordenadas = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }

    public void Recalculate(Premissas _premissas, GasNatural _gas)
    {
        heatRate = _premissas.fatorBtuKwh / (eficiencia / 100);
        demandaDiaPcs = potencia * 24 * heatRate / 1000000;
        demandaDiaPci = demandaDiaPcs / _gas.rel_pciPcs;
        demandaDiaM3 = demandaDiaPci * _gas.m3BtuBasePci;
        demandaAnual = demandaDiaPci * 365 * despacho * 0.01f;
    }
}

[Serializable]
public class PontosDeEntrega
{
    public List<PontoDeEntrega> pontosDeEntrega;
}

[Serializable]
public class PontoDeEntrega
{
    public string nome;
    public string categoria;
    public Coordenadas coordenadas;
    public string GasodutoTransporte;
    public double[] coluna_de = new double[10] { 0, 15001, 45001, 300001, 900001, 3000001, 900001, 15000001, 30000001, 60000001 };
    public double[] coluna_ate = new double[10] { 15000, 45000, 300000, 900000, 3000000, 9000000, 15000000, 30000000, 60000000, 90000000 };
    public double[] coluna_prc = new double[10] { 4024.85, 4419.21, 6601.02, 12929.26, 36872.62, 107238.73, 166904.05, 180743.63, 199290.12, 284700.18 };
    public double[] coluna_puc = new double[10] { 0.2254, 0.1992, 0.1506, 01296, 0.1029, 0.0795, 0.061, 0.0506, 0.0378, 0.0264 };
    public double valorMalha = 3.88327;
    public PontoDeEntrega()
    {
        coordenadas = new Coordenadas();
    }

    public void SetCoordenadas(double _lon, double _lat)
    {
        coordenadas = new Coordenadas
        {
            lon = _lon,
            lat = _lat
        };
    }
}

[Serializable]
public class Coordenadas
{
    public double lat;
    public double lon;
}

[Serializable]
public class Custo
{
    public string tipo;
    public string moeda;
    public double valor;
}

[Serializable]
public class Premissas
{
    public double fatorJouleKcal;
    public double fatorBtuJoule;
    public double fatorBtuKwh;
    public double cotacaoDolar;
    public double tma;
    public int periodoAmortizacao;
    public double frete;
    public Premissas()
    {
        fatorJouleKcal = 4184;
        fatorBtuJoule = 1055.06f;
        fatorBtuKwh = 1000 * 60 * 60 / fatorBtuJoule;
        frete = 5.5f;
    }

    public Premissas(double _cotacao, double _tma, int _amortizacao, double _frete)
    {
        fatorJouleKcal = 4184;
        fatorBtuJoule = 1055.06f;
        fatorBtuKwh = 1000 * 60 * 60 / fatorBtuJoule;
        cotacaoDolar = _cotacao;
        tma = _tma;
        periodoAmortizacao = _amortizacao;
        frete = _frete;
    }

    public void Set(double _cotacao, double _tma, int _amortizacao, double _frete)
    {
        cotacaoDolar = _cotacao;
        tma = _tma;
        periodoAmortizacao = _amortizacao;
        frete = _frete;
    }

}

[Serializable]
public class GasNatural
{

    public double pcs;
    public double pci;
    public double rel_pciPcs;
    public double m3BtuBasePci;
    public double m3BtuBasePcs;
    public double taxaCompressao;

    public GasNatural(double _pci, double _pcs, double _taxa)
    {
        pci = _pci;
        pcs = _pcs;
        rel_pciPcs = pci / pcs;
        m3BtuBasePcs = (1000000) / (pcs * 4184 / 1055.06f);
        m3BtuBasePci = m3BtuBasePcs / rel_pciPcs;
        taxaCompressao = _taxa;

    }

    public GasNatural(double _pci, double _pcs, double _taxa, Premissas _premissas)
    {
        Premissas premissas = _premissas;
        pci = _pci;
        pcs = _pcs;
        rel_pciPcs = pci / pcs;
        m3BtuBasePcs = (1000000) / (pcs * premissas.fatorJouleKcal / premissas.fatorBtuJoule);
        m3BtuBasePci = m3BtuBasePcs / rel_pciPcs;
        taxaCompressao = _taxa;
    }

    public void SetPciAndPcs(double _pci, double _pcs, double _taxa)
    {
        pci = _pci;
        pcs = _pcs;
        rel_pciPcs = pci / pcs;
        m3BtuBasePcs = (1000000) / (pcs * 4184 / 1055.06f);
        m3BtuBasePci = m3BtuBasePcs / rel_pciPcs;
        taxaCompressao = _taxa;
    }

    public void SetPciAndPcs(double _pci, double _pcs, double _taxa, Premissas _premissas)
    {
        Premissas premissas = _premissas;
        pci = _pci;
        pcs = _pcs;
        rel_pciPcs = pci / pcs;
        m3BtuBasePcs = (1000000) / (pcs * premissas.fatorJouleKcal / premissas.fatorBtuJoule);
        m3BtuBasePci = m3BtuBasePcs / rel_pciPcs;
        taxaCompressao = _taxa;
    }
}

[Serializable]
public class GasodutoIntegrador
{
    public string nome;
    public string fsruOrigem;
    public string terminalRegasOrigem;
    public string peDestino;
    public double custoBase;
    public double comprimento;
    public double diametro;


    public List<Custo> custos;

    public GasodutoIntegrador()
    {
        comprimento = 0;
        custoBase = 0;
        diametro = 0;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public GasodutoIntegrador(string _nome, double _comprimento)
    {
        nome = _nome;
        comprimento = _comprimento;
        custoBase = 0;
        diametro = 0;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public void Set(double _custoBase, double _diametro, Premissas _premissas)
    {
        custoBase = _custoBase;
        diametro = _diametro;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);

    }

    //TODO: VERIFICAR SE ESSE CONSTRUTOR É NECESSÁRIO
    public GasodutoIntegrador(double _custoBase, double _comprimento, double _diametro, Premissas _premissas)
    {
        custoBase = _custoBase;
        comprimento = _comprimento;
        diametro = _diametro;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public GasodutoIntegrador(string _nome, double _custoBase, double _comprimento, double _diametro, Premissas _premissas)
    {
        nome = _nome;
        custoBase = _custoBase;
        comprimento = _comprimento;
        diametro = _diametro;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public void Recalculate(Premissas _premissas)
    {

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        opex.valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor);

        bool hasOpex = false;
        bool hasCapex = false;
        foreach (Custo c in custos)
        {
            if (!string.IsNullOrEmpty(c.tipo))
            {
                if (c.tipo == opex.tipo)
                {
                    c.valor = opex.valor;
                    hasOpex = true;
                }
                else if (c.tipo == capex.tipo)
                {
                    c.valor = capex.valor;
                    hasCapex = true;
                }
            }
        }

        if (!hasOpex)
            custos.Add(opex);
        if (!hasCapex)
            custos.Add(capex);


    }

    public static double PMT(double yearlyInterestRate, int totalNumberOfMonths, double loanAmount)
    {
        var n = totalNumberOfMonths;
        var r = yearlyInterestRate * 0.01f;
        var pmt = loanAmount * r / (1 - Math.Pow(1 + r, -n));
        return pmt;
    }
}

[Serializable]
public class GasodutoCriogenico
{
    public string nome;
    public string fsuOrigem;
    public string terminalCargaDestino;
    public string terminalRegasDestino;
    public double custoBase;
    public double comprimento;
    public double diametro;
    public List<Custo> custos;

    public GasodutoCriogenico()
    {
        comprimento = 0;
        custoBase = 0;
        diametro = 0;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public GasodutoCriogenico(string _nome, double _comprimento)
    {
        nome = _nome;
        comprimento = _comprimento;
        custoBase = 0;
        diametro = 0;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = 0,
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public void Set(double _custoBase, double _diametro, Premissas _premissas)
    {
        custoBase = _custoBase;
        diametro = _diametro;
        custos = new List<Custo>();
        custos.Clear();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);

    }

    //TODO: VERIFICAR SE ESSE CONSTRUTOR É NECESSÁRIO
    public GasodutoCriogenico(double _custoBase, double _comprimento, double _diametro, Premissas _premissas)
    {
        custoBase = _custoBase;
        comprimento = _comprimento;
        diametro = _diametro;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public GasodutoCriogenico(string _nome, double _custoBase, double _comprimento, double _diametro, Premissas _premissas)
    {
        nome = _nome;
        custoBase = _custoBase;
        comprimento = _comprimento;
        diametro = _diametro;
        custos = new List<Custo>();
        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor),
            moeda = "dolar"
        };

        custos.Add(capex);
        custos.Add(opex);
    }

    public void Recalculate(Premissas _premissas)
    {
        Custo opex = new Custo()
        {
            tipo = "opex",
            valor = 0,
            moeda = "dolar"
        };

        Custo capex = new Custo()
        {
            tipo = "capex",
            valor = (double)(custoBase * comprimento * 1000 * diametro),
            moeda = "dolar"
        };

        opex.valor = (double)PMT(_premissas.tma, _premissas.periodoAmortizacao, capex.valor);

        bool hasOpex = false;
        bool hasCapex = false;
        foreach (Custo c in custos)
        {
            if (!string.IsNullOrEmpty(c.tipo))
            {
                if (c.tipo == opex.tipo)
                {
                    c.valor = opex.valor;
                    hasOpex = true;
                }
                else if (c.tipo == capex.tipo)
                {
                    c.valor = capex.valor;
                    hasCapex = true;
                }
            }
        }

        if (!hasOpex)
            custos.Add(opex);
        if (!hasCapex)
            custos.Add(capex);
    }

    public static double PMT(double yearlyInterestRate, int totalNumberOfMonths, double loanAmount)
    {
        var n = totalNumberOfMonths;
        var r = yearlyInterestRate * 0.01f;
        var pmt = loanAmount * r / (1 - Math.Pow(1 + r, -n));
        return pmt;
    }
}

[Serializable]
public class SubestacaoEnergia
{
    public string nome;
    public double tensao;
    public string categoria;
    public Coordenadas coordenadas;
}

[Serializable]
public class GasodutoDistribuicao
{
    public string nome;

    public string peOrigem;
    public string usinaDestino;
    public double consumoM3Mes;
    public double puc;
    public double prc;
    public double custoAnualPrc;
    public double custoAnualPuc;

    public Custo opex;

    public GasodutoDistribuicao(string _nome, Usina _usina, PontoDeEntrega _pe, GasNatural _gas, Premissas _premissas)
    {
        nome = _nome;
        usinaDestino = _usina.nome;

        consumoM3Mes = _usina.demandaDiaM3 * 30;

        for (int x = 0; x < _pe.coluna_de.Length; x++)
        {
            if (consumoM3Mes >= _pe.coluna_de[x] && consumoM3Mes <= _pe.coluna_ate[x])
            {
                prc = _pe.coluna_prc[x];
                puc = _pe.coluna_puc[x];
                break;
            }
        }


        custoAnualPrc = prc * 12;
        custoAnualPuc = _usina.demandaAnual * puc / _gas.m3BtuBasePci;


        opex = new Custo()
        {
            tipo = "opex",
            valor = (custoAnualPrc + custoAnualPuc) / _premissas.cotacaoDolar,
            moeda = "dolar"
        };


    }

    public void Set(Usina _usina, PontoDeEntrega _pe, GasNatural _gas, Premissas _premissas)
    {
        usinaDestino = _usina.nome;
        consumoM3Mes = _usina.demandaDiaM3 * 30;

        for (int x = 0; x < _pe.coluna_de.Length; x++)
        {
            if (consumoM3Mes >= _pe.coluna_de[x] && consumoM3Mes <= _pe.coluna_ate[x])
            {
                prc = _pe.coluna_prc[x];
                puc = _pe.coluna_puc[x];
                break;
            }
        }

        custoAnualPrc = prc * 12;
        custoAnualPuc = _usina.demandaAnual * puc / _gas.m3BtuBasePci;

        opex = null;
        opex = new Custo()
        {
            tipo = "opex",
            valor = (custoAnualPrc + custoAnualPuc) / _premissas.cotacaoDolar,
            moeda = "dolar"
        };
    }

    public void Recalculate(Usina _usina, PontoDeEntrega _pe, Premissas _premissas, GasNatural _gas)
    {
        consumoM3Mes = _usina.demandaDiaM3 * 30;

        for (int x = 0; x < _pe.coluna_de.Length; x++)
        {
            if (consumoM3Mes >= _pe.coluna_de[x] && consumoM3Mes <= _pe.coluna_ate[x])
            {
                prc = _pe.coluna_prc[x];
                puc = _pe.coluna_puc[x];
                break;
            }
        }

        custoAnualPrc = prc * 12;
        custoAnualPuc = _usina.demandaAnual * puc / _gas.m3BtuBasePci;

        opex = new Custo()
        {
            tipo = "opex",
            valor = (custoAnualPrc + custoAnualPuc) / _premissas.cotacaoDolar,
            moeda = "dolar"
        };

    }
}