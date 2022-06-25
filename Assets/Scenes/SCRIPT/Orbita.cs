using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Orbita : MonoBehaviour
{

    private float eskala = 0.0001f;

    private float h = 0.0001f;

    private double C = 1.7554962315534863e10;
    private Vector3 q0;
    private Vector3 v0;
    private Vector3 v_normal;

    private double mu = 398600f;

    private double[] qlag  = new double[3];
    private double[] vlag  = new double[3];
    private double[] rlag  = new double[3];

    private float rx;
    private float ry;
    private float rz;

    private Sateliteak satelitea;
    private KamerenKontrola kamerenKontrola;
    public GameObject kam;

    private int kKont = 0;
    private int kameraKont;
    private int momentukoDisplay;
    private int kameraID;
    private int kameraLag; 

    private bool xagua = false;
    private float sens = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
        satelitea = GameObject.FindObjectOfType<Sateliteak> ();
        kamerenKontrola = GameObject.FindObjectOfType<KamerenKontrola> ();

        q0 = satelitea.getQ();
        v0 = satelitea.getV();

        transform.position = eskala * q0;
        transform.forward = v0;
        
        qlag[0] = q0[0];
        qlag[1] = q0[1];
        qlag[2] = q0[2];
        vlag[0] = v0[0];
        vlag[1] = v0[1];
        vlag[2] = v0[2];

        kameraID = satelitea.getSateliteKont() + 2;
       
    }

    // Update is called once per frame
    void Update()
    {
        
        /*############### TAYLOR METODOA ERABILTZEKO ################*/
        (qlag, vlag) = taylor(qlag, vlag, h, 10);
        /*################# RK4 METODOA ERABILTZEKO #################*/
        // (qlag, vlag) = RK4(qlag, vlag, h); 
        /*############### RALSTON METODOA ERABILTZEKO ###############*/
        // (qlag, vlag) = ralston(qlag, vlag, h); 
        
        Vector3 q = new Vector3((float)qlag[0], (float)qlag[1], (float)qlag[2]);
        Vector3 v = new Vector3((float)vlag[0], (float)vlag[1], (float)vlag[2]);

        //Posizioa aldatu
        transform.position = eskala * q;

        //Satelitea biratu
        v_normal = bektoreLagKalkulatu(q, v);
        transform.rotation = Quaternion.LookRotation(v, v_normal);

        //Kameren kontrola:
        if (Input.GetKeyDown(KeyCode.K) || OVRInput.GetDown(OVRInput.Button.Three)){
            kameraKont = satelitea.getSateliteKont()+3;
            kKont = kamerenKontrola.getKont();
            kameraLag = kKont % kameraKont;
        }
        

        if (kameraID == kameraLag){
            if (Input.GetKeyDown(KeyCode.X)){
                xagua = !xagua;
            }
            if (xagua){
                kam.transform.Rotate(Vector3.right *  Input.GetAxis("Mouse Y") * sens);
                kam.transform.Rotate(Vector3.up *  Input.GetAxis("Mouse X") * sens);
            }else{
                kam.transform.LookAt(transform.position, -transform.forward);
            }
            
            kam.transform.position = transform.position + transform.up*(float)0.5;
        }

    }

    /*###############################################################################*/
    /*######################## V(q) FUNTZIOAREN GRADIENTEA ##########################*/
    /*###############################################################################*/

    private double[] gradV(double[] q){

        double[] gradVq = new double[3]; 

        double x = q[0];
        double y = q[1];
        double z = q[2];
        double lag1 = Math.Pow(x,2);
        double lag2 = Math.Pow(y,2);
        double lag3 = Math.Pow(z,2);
        double r2 = lag1 + lag2 + lag3;
        double r = Math.Sqrt(r2);
        r2 = Math.Pow(r2, 2);
        double sinth = z/r;
        double aux1 = 1.5 * C / (r * r2);
        double aux2 = 5 * Math.Pow(sinth, 2);
        double aux3 = aux1 * (1 - aux2);

        gradVq[0] = aux3*x;
        gradVq[1] = aux3*y;
        gradVq[2] = aux1*(3 - aux2)*z;

        return gradVq;

    }

    /*###############################################################################*/
    /*############### V(q) FUNTZIOAREN GRADIENTEA POLINOMIOEKIN #####################*/
    /*###############################################################################*/

    private (double[], double[], double[]) gradV2(double[] qx, double[] qy, double[] qz, int ordena){

        double[] gradVqx;
        double[] gradVqy;
        double[] gradVqz;

        double[] x = qx;
        double[] y = qy;
        double[] z = qz;

        double[] lagr2 = polinomioenGehiketa(polinomioenBiderketa(x, x, ordena), polinomioenBiderketa(y, y, ordena), ordena);
        double[] r2 = polinomioenGehiketa(lagr2, polinomioenBiderketa(z, z, ordena), ordena);

        double[] r = polinomioenBerreketa(r2, (float)0.5, ordena);

        r2 = polinomioenBerreketa(r2, 2, ordena);
        double[] sinth = polinomioenZatiketa(z, r, ordena);

        double[] aux1 = polinomioBiderK(polinomioenBerreketa(polinomioenBiderketa(r, r2, ordena) , -1, ordena) , 1.5*C, ordena);
        
        double[] aux2 = polinomioBiderK(polinomioenBiderketa(sinth, sinth, ordena) , 5, ordena);
        
        double[] aux3 = polinomioenKenketa(aux1, polinomioenBiderketa(aux1, aux2, ordena) , ordena);

        gradVqx = polinomioenBiderketa(aux3, x, ordena);
        gradVqy = polinomioenBiderketa(aux3, y, ordena);
        double[] lag = polinomioenKenketa(polinomioBiderK(aux1, 3, ordena) , polinomioenBiderketa(aux1, aux2, ordena) , ordena);
        gradVqz = polinomioenBiderketa(lag, z, ordena);

        return (gradVqx, gradVqy, gradVqz);

    }

    /*###############################################################################*/
    /*######################## EKUAZIO DIFERENTZIALEN KALKULUA ######################*/
    /*###############################################################################*/

    private (double[], double[]) edf(double[] q, double[] v){


        double[] dq = new double[3];
        double[] dv = new double[3];
        double[] gradVq = gradV(q);

        double r2 = q[0]*q[0] + q[1]*q[1] + q[2]*q[2];
        double r = Math.Sqrt(r2);
        dq = doubleBiderK(r, v);
        double[] lag1 = new double[3];
        double[] lag2 = new double[3];
        double lag3 = -mu/r2;
        lag1 = doubleBiderK(lag3, q);
        lag2 = doubleBiderK(r, gradVq);
        dv = doubleKenDouble(lag1, lag2);

        return(dq, dv);

    }

    /*###############################################################################*/
    /*################ EKUAZIO DIFERENTZIALEN KALKULUA POLINOMIOEKIN ################*/
    /*###############################################################################*/

    private (double[], double[], double[], double[], double[], double[]) edf2(double[] qx, double[] qy, double[] qz, double[] vx, double[] vy, double[] vz, int ordena){

        double[] dqxlag;
        double[] dqylag;
        double[] dqzlag;
        double[] dvxlag;
        double[] dvylag;
        double[] dvzlag;

        double[] gradVqx;
        double[] gradVqy;
        double[] gradVqz;

        double[] r2lag = polinomioenGehiketa(polinomioenBiderketa(qx, qx, ordena), polinomioenBiderketa(qy, qy, ordena), ordena);
        double[] r2 = polinomioenGehiketa(r2lag, polinomioenBiderketa(qz, qz, ordena), ordena);

        double[] r = polinomioenBerreketa(r2, (float)0.5, ordena);

        dqxlag = polinomioenBiderketa(r, vx, ordena);
        dqylag = polinomioenBiderketa(r, vy, ordena);
        dqzlag = polinomioenBiderketa(r, vz, ordena);

        double[] lag1 = polinomioBiderK(polinomioenBerreketa(r2, -1, ordena), -mu, ordena);
        double[] lag1x = polinomioenBiderketa(lag1, qx, ordena);
        double[] lag1y = polinomioenBiderketa(lag1, qy, ordena);
        double[] lag1z = polinomioenBiderketa(lag1, qz, ordena);

        (gradVqx, gradVqy, gradVqz) = gradV2(qx, qy, qz, ordena);

        dvxlag = polinomioenKenketa(lag1x, polinomioenBiderketa(r, gradVqx, ordena), ordena);
        dvylag = polinomioenKenketa(lag1y, polinomioenBiderketa(r, gradVqy, ordena), ordena);
        dvzlag = polinomioenKenketa(lag1z, polinomioenBiderketa(r, gradVqz, ordena), ordena);



        return(dqxlag, dqylag, dqzlag, dvxlag, dvylag, dvzlag);


    }

    /*###############################################################################*/
    /*#################### TAYLOR METODOA APLIKATZEKO FUNTZIOA ######################*/
    /*###############################################################################*/

    private (double[], double[]) taylor(double[] q, double[] v, double h, int ordena){

        double[] qx = new double[ordena+1];
        double[] qy = new double[ordena+1];
        double[] qz = new double[ordena+1];
        double[] vx = new double[ordena+1];
        double[] vy = new double[ordena+1];
        double[] vz = new double[ordena+1];

        double[] dqx = new double[ordena+1];
        double[] dqy = new double[ordena+1];
        double[] dqz = new double[ordena+1];
        double[] dvx = new double[ordena+1];
        double[] dvy = new double[ordena+1];
        double[] dvz = new double[ordena+1];
        
        double[] dq = new double[3];
        double[] dv = new double[3];

        qx[0] = q[0];
        qy[0] = q[1];
        qz[0] = q[2];

        vx[0] = v[0];
        vy[0] = v[1];
        vz[0] = v[2];

       
        for (int i = 1; i<=ordena; i++){
            (dqx, dqy, dqz, dvx, dvy, dvz) = edf2(qx, qy, qz, vx, vy, vz, i);

            qx[i] = (dqx[i-1]/i);
            qy[i] = (dqy[i-1]/i);
            qz[i] = (dqz[i-1]/i);
            vx[i] = (dvx[i-1]/i);
            vy[i] = (dvy[i-1]/i);
            vz[i] = (dvz[i-1]/i);

        }

        dq[0] = polinomioenKalulua(qx, h, ordena+1);
        dq[1] = polinomioenKalulua(qy, h, ordena+1);
        dq[2] = polinomioenKalulua(qz, h, ordena+1);
        dv[0] = polinomioenKalulua(vx, h, ordena+1);
        dv[1] = polinomioenKalulua(vy, h, ordena+1);
        dv[2] = polinomioenKalulua(vz, h, ordena+1);

        return (dq, dv);
    }

    /*###############################################################################*/
    /*##################### RK4 METODOA APLIKATZEKO FUNTZIOA ########################*/
    /*###############################################################################*/

    private (double[], double[]) RK4(double[] q, double[] v, double h){

        double[] q0 = q;
        double[] v0 = v;

        double[] lag1q, lag1v, lag2q, lag2v, lag3q, lag3v, lag4q, lag4v, q1, v1, lag5q, lag5v;
        (lag1q, lag1v) = edf(q0, v0);
        
        (lag2q, lag2v) = edf(doubleGehiDouble(q0, doubleBiderK(h, doubleBiderK(0.5, lag1q))), doubleGehiDouble(v0, doubleBiderK(h, doubleBiderK(0.5, lag1v))));
        
        (lag3q, lag3v) = edf(doubleGehiDouble(q0, doubleBiderK(h, doubleBiderK(0.5, lag2q))), doubleGehiDouble(v0, doubleBiderK(h, doubleBiderK(0.5, lag2v))));
        
        (lag4q, lag4v) = edf(doubleGehiDouble(q0, doubleBiderK(h, lag3q)), doubleGehiDouble(v0, doubleBiderK(h, lag3v)));

        lag5q = doubleGehi4Double(lag1q, doubleBiderK(2, lag2q), doubleBiderK(2, lag3q), lag4q);
        lag5v = doubleGehi4Double(lag1v, doubleBiderK(2, lag2v), doubleBiderK(2, lag3v), lag4v);
        lag5q = doubleBiderK(h, lag5q);
        lag5v = doubleBiderK(h, lag5v);
        lag5q = doubleZatiK(6, lag5q);
        lag5v = doubleZatiK(6, lag5v);

        q1 = doubleGehiDouble(q0, lag5q);
        v1 = doubleGehiDouble(v0, lag5v);
        
        return(q1, v1);

    }

    /*###############################################################################*/
    /*################### RALSTON METODOA APLIKATZEKO FUNTZIOA ######################*/
    /*###############################################################################*/

    private (double[], double[]) ralston(double[] q, double[] v, double h){

        double[] q0 = q;
        double[] v0 = v;

        double[] k1q, k1v, k2q, k2v, q1, v1, lagq, lagv;

        (k1q, k1v) = edf(q0, v0);
        double hlag = 3*h/4;
        (k2q, k2v) = edf(doubleGehiDouble(q0, doubleBiderK(hlag, k1q)), doubleGehiDouble(v0, doubleBiderK(hlag, k1v)));

        lagq = doubleGehiDouble(k1q, doubleBiderK(2, k2q));
        lagv = doubleGehiDouble(k1v, doubleBiderK(2, k2v));

        double hlag2 = h/3;
        q1 = doubleGehiDouble(q0, doubleBiderK(hlag2, lagq));
        v1 = doubleGehiDouble(v0, doubleBiderK(hlag2, lagv));

        return (q1, v1);

    }

    private int faktorial(int n){
        if (n == 0){  
            return 1;  
        }else{  
            return(n * faktorial(n-1));
        }
    }

    private Vector3 bektoreLagKalkulatu(Vector3 q, Vector3 v){

        Vector3 z_s = -Vector3.Normalize(v);
        Vector3 x_s = Vector3.Normalize(Vector3.Cross(q, z_s));
        return Vector3.Cross(z_s, x_s);

    }

    /*###############################################################################*/
    /* DOUBLE[] MOTATAKO PARAMETROEN ARTEKO ERAGIKETAK EGITEKO FUNTZIO LAGUNTZAILEAK */
    /*###############################################################################*/

    private double[] doubleBiderK(double k, double[] v){
        double[] emaitza = new double[3];
        emaitza[0] = k * v[0];
        emaitza[1] = k * v[1];
        emaitza[2] = k * v[2];
        return emaitza;        
    }

    private double[] doubleZatiK(double k, double[] v){
        double[] emaitza = new double[3];
        emaitza[0] = v[0] / k;
        emaitza[1] = v[1] / k;
        emaitza[2] = v[2] / k;
        return emaitza;        
    }

    private double[] doubleGehiDouble(double[] d1, double[] d2){
        double[] emaitza = new double[3];
        emaitza[0] = d1[0] + d2[0];
        emaitza[1] = d1[1] + d2[1];
        emaitza[2] = d1[2] + d2[2];
        return emaitza;        
    }

    private double[] doubleKenDouble(double[] d1, double[] d2){
        double[] emaitza = new double[3];
        emaitza[0] = d1[0] - d2[0];
        emaitza[1] = d1[1] - d2[1];
        emaitza[2] = d1[2] - d2[2];
        return emaitza;        
    }

    private double[] doubleGehi4Double(double[] d1, double[] d2, double[] d3, double[] d4){
        double[] emaitza = new double[3];
        emaitza[0] = d1[0] + d2[0] + d3[0] + d4[0];
        emaitza[1] = d1[1] + d2[1] + d3[1] + d4[1];
        emaitza[2] = d1[2] + d2[2] + d3[2] + d4[2];
        return emaitza;        
    }

    /*#############################################################*/
    /* POLINOMIOEN ARTEKO ERAGIKETAK EGITEKO FUNTZIO LAGUNTZAILEAK */
    /*#############################################################*/

    private double[] polinomioenGehiketa(double[] p1, double[] p2, int luzera){
        double[] emaitza = new double[luzera];
        for (int i = 0; i<luzera; i++){
            emaitza[i] = p1[i]+p2[i];
        }
        return emaitza;
    }

    private double[] polinomioenKenketa(double[] p1, double[] p2, int luzera){
        double[] emaitza = new double[luzera];
        for (int i = 0; i<luzera; i++){
            emaitza[i] = p1[i]-p2[i];
        }
        return emaitza;
    }

    private double[] polinomioenBiderketa(double[] p1, double[] p2, int luzera){
        double[] emaitza = new double[luzera];
        double lag = 0;
        emaitza[0] = p1[0] * p2[0];
        for (int i = 1; i<luzera; i++){
            lag = 0;
            for (int j = 0; j<=i;j++){
                lag += p1[j] *p2[i-j];
            }
            emaitza[i]=lag;
        }
        return emaitza;
    }

    private double[] polinomioenZatiketa(double[] p1, double[] p2, int luzera){
        double[] emaitza = new double[luzera];
        for (int i = 0; i<luzera; i++){
            double lag1 = 1/p2[0];
            double lag2 = p1[i];
            double lag3 = 0;
            for (int j = 0; j<i;j++){
                lag3 += emaitza[j]*p2[i-j];
            }
            emaitza[i]=lag1 * (lag2 - lag3);
        }
        return emaitza;
    }

    private double[] polinomioenBerreketa(double[] p, float ber, int luzera){
        double[] emaitza = new double[luzera];

        emaitza[0] = Math.Pow(p[0], ber);
        for (int i = 1; i<luzera;i++){
            double lag = 0;
            for (int j = 0; j<i;j++){
                lag += (((ber*(i-j))-j) * p[i-j]) * emaitza[j];
            }
            emaitza[i] = lag/(i*p[0]);
        }

        return emaitza;
    }

    private double polinomioenKalulua(double[] p, double h,int luzera){
        double emaitza=0;
        for (int i = 0; i<luzera;i++){
            double hlag = Math.Pow(h, i);
            emaitza += p[i] * hlag;
        } 
        return emaitza;

    }

    private double[] polinomioBiderK(double[] p, double k, int luzera){
        double[] emaitza = new double[luzera];

        for (int i = 0; i<luzera; i++){
            emaitza[i] = p[i]*k;
        }
        return emaitza;
    }





}



