using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilisticCalculation : MonoBehaviour
{

    const double NAVC = 4;// navigational constant 4
    const double PTHMAX = 30; //maximum pitch acceleration in G
    const double YAWMAX = 30; //maximum yaw acceleration 30 G
    const double TIMRNG = 100; //distance to fuse warhead at 100 meters;
    const double SEEKER = 15; //seeker limitation .261798 radians(15 degrees);
    const double MXTIME = 8; //time of missile burn 8 seconds;
     
    const double MINTRT = 0; //Thrust after motor burnout 0 N;
    const double G = 9.8; //Gravitational constant 9.8 meters per second2;
    const double OPVEL = 700; //Optimal velocity of missile 700 meter per second;

    double MAXACCEL = 100;
    double DETRNG = 10; //detonation range(hit score) 10 meters;
    double MASS = 56.7f;
    double MAXWGT = 56.7F; //maximum weight of missile 56.7 kilograms;
    double MINWGT = 22.7; //minimum weight of missile 22.7 kilograms;
    double MOTORBURNTIME = 9;
    double MAXTRT = 6800; //Thrust during motor burn 6800 N;
    
    public double ConvertToRadians(double angle)
    {
        return (Mathf.PI / 180) * angle;
    }

    private void ExtractConstantsFromMissileData(MissileData missileData)
    {
        MAXACCEL =                          missileData.MaximumAcceleration;
        MAXWGT = MASS =                     missileData.WeightAtLaunch;
        MINWGT =                            missileData.WeightAtBurnout;
        MOTORBURNTIME =                     missileData.TimeOfMotorBurn;
        MAXTRT =                            missileData.Thrust;
        DETRNG =                            missileData.Range;
    }

    private float GetHitProbability(GameObject ownShip, GameObject target, double missileRange)
    {
        double totalTerrainLength = MathCalculations.MaxBounds()[0];

        float missileUnityUnits = (float)(missileRange / totalTerrainLength) * 2f;
        float distance = Vector3.Distance(ownShip.transform.position, target.transform.position);

        if (distance <= missileUnityUnits)
        {
            Vector3 targetDir = target.transform.position - ownShip.transform.position;
            float angle = Vector3.Angle(targetDir, ownShip.transform.forward);

            //Is in semi sphere cone
            if (angle > 90f)
            {
                float directionalProbability = angle / 180f;
                float distanceProbability = 1 - (distance / missileUnityUnits);
                return distanceProbability * directionalProbability;
            }

        }
        return 0f;
    }

    public bool CalculateIsTargetHit(GameObject ownShip, GameObject targetAircraft, MissileData missileData, ref float probability)
    {
        probability = GetHitProbability(ownShip.transform.GetChild(0).gameObject, targetAircraft.transform.GetChild(0).gameObject, missileData.Range);
        ExtractConstantsFromMissileData(missileData);
        Vector3 ownerPosition = ownShip.transform.localPosition;
        Vector3 ownerDirection = ownShip.transform.localRotation.eulerAngles;
        //Velocity, Pitch Rate, Yaw Rate, Pitch and Yaw
        float velcty, pitchr, yawr, pitch, yaw;
        //Position of missile, weight of missile, range (distance of missile and target)
        //Get this value from data table
        float x, y, z, range;
       
        range = -1;
        int loop1, loop2;
        //Target's x y z
        float tarx, tary, tarz;

        //Iterations per second
        int itmin;
        itmin = 128;


        //Pitch signal, Yaw signal
        float pthsig, yawsig;


        int hit, miss;
        //Last iteration position
        float otarx, otary, otarz;

        velcty = 200.0f;
        pitchr = 0.0f;
        yawr    = 0.0f;
        pthsig = 0.0f;
        yawsig = 0.0f;
        pitch = ownerDirection.x * Mathf.Deg2Rad;
        yaw = ownerDirection.y * Mathf.Deg2Rad;

       
        x = MathCalculations.ConvertUnityToMeters(ownerPosition.x,false);
        y = MathCalculations.ConvertUnityToMeters(ownerPosition.y,true);
        z = MathCalculations.ConvertUnityToMeters(ownerPosition.z,true);


        tarx = 0;
        tary = 0;
        tarz = 0;
        Gettar(ref tarx, ref tary, ref tarz, targetAircraft);

        hit = 0;
        miss = 0;

        //Lets assume this loop1 = seconds
        for (loop1 = 1; loop1 <= 30; loop1++)
        {
            //For each second do 128 times
            for (loop2 = 1; loop2 <= itmin; loop2++)
            {
                otarx = tarx;
                otary = tary;
                otarz = tarz;
                Gettar(ref tarx, ref tary, ref tarz, targetAircraft);
                Calculations(ref velcty, ref pitchr, ref pthsig, ref yawr, ref yawsig
                    , ref pitch, ref yaw, ref x, ref y, ref z, ref tarx, ref tary, ref tarz, ref loop1,
                    ref loop2, ref itmin, ref hit, ref miss, ref otarx, ref otary, ref otarz, ref range);
                if (hit == 1)
                {
                    Debug.Log(string.Format("Target X = {0}, Target Y = {1}, Target Z = {2}", tarx, tary, tarz));
                    return true;
                }
                if (miss == 1)
                {
                    return false;
                }
                //Continue if still indeterminate

            }
            
        }
    
        return (hit == 1);
    }



    /****************************************************************
    * Name - Subroutine Calculations
    * Purpose - Implements a missile simulation for the
    * Paladin tactical decision generator.
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------------
    * VELCTY REAL meters/second
    * PITCHR REAL radians/second
    * PTHSIG REAL radians/second**2
    * YAWR REAL radians/second
    * YAWSIG REAL radians/second**2
    * PITCH REAL radians
    * YAW REAL radians
    * X REAL earth-based x in meters
    * Y REAL earth-based y in meters
    * Z REAL earth-based z in meters
    * MASS REAL pounds
    * TARX REAL earth-based x in meters
    * TARY REAL earth-based y in meters
    * TARZ REAL earth-based z in meters
    * SEC INTEGER seconds
    * ITS INTEGER iteration # in current second
    * ITMIN INTEGER # of iterations per second
    * HIT INTEGER 1 or 0
    * MISS INTEGER 1 or 0
    * OTARX REAL earth-based x in meters
    * OTARY REAL earth-based y in meters
    * OTARZ REAL earth-based z in meters
    * RANGE REAL meters
    ****************************************************************/
    private void Calculations(ref float velcty, ref float pitchr, ref float pthsig, ref float yawr, ref float yawsig, ref float pitch, ref float yaw, ref float x, ref float y, ref float z, ref float tarx, ref float tary, ref float tarz, ref int sec, ref int its, ref int itmin, ref int hit, ref int miss, ref float otarx, ref float otary, ref float otarz, ref float range)
    {

        float thrust, drag;
        float accelr, ptchrd, yawrd, pitchd, yawd;
        float xd, yd, zd;

        //float cmpdis, cmpsig;
        //float cmpdrg, cmptht, accel;
        //float pthacc, yawacc, cngpth, cngyaw;
        //float cngx, cngy, cngz;


        float currr, currx, curry, currz;
        float newr, newx, newy, newz;
        float resr, resx, resy, resz;
        float clsvel;
        //, cmpop, cmpoy;
        float linex = 0, liney = 0, linez = 0;
        float pitcho, yawo;
        float imptme, intrvl;



        float seeker;
        seeker = (float)ConvertToRadians(30); // SHOULD BE 0.523596f;

        float xcom, yzcom;
        float lnang;

        //rate of change of time
        intrvl = 1 / (float)(itmin);

        //Difference in X Y Z axis (otarx for old target x axis)
        currx = (otarx - x);
        curry = (otary - y);
        currz = (otarz - z);

        //Range between missile and target for previous iteration
        currr = CMPDIS(currx, curry, currz);

        //Having lost thrust, the missile will begin to quickly decelerate
        thrust = CMPTHT(sec, (float)MAXTRT, (float)MOTORBURNTIME, (float)MINTRT);

        //Missile drag for current iteration
        drag = CMPDRG(velcty, pitchr, yawr);

        //Missile acceleration - taking into account thrust and drag
        accelr = ACCEL(thrust, drag, pitch, (float)MASS);

        //Pitch and Yaw accelerations based on signals from control surface (if provided)
        ptchrd = PTHACC(pthsig, pitchr);
        yawrd = YAWACC(yawsig, yawr);

        //Pitch and Yaw (angle velocity)_
        pitchd = CNGPTH(pitchr, velcty, pitch);
        yawd = CNGYAW(yawr, velcty, pitch);

        //Velocity for X Y and Z axis is calculated using pitch and yaw angles
        xd = CNGX(velcty, pitch, yaw);
        yd = CNGY(velcty, pitch, yaw);
        zd = CNGZ(velcty, pitch);

        //Now lets calculate velocity
        velcty = velcty + (accelr * 1 / itmin);

        //Calculate pitch and yaw rates
        pitchr = pitchr + (ptchrd * 1 / itmin);
        yawr = yawr + (yawrd * 1 / itmin);

        //Update pitch and yaw
        pitch = pitch + (pitchd * 1 / itmin);
        yaw = yaw + (yawd * 1 / itmin);

        //X Y Z position for current iterations
        x = x + (xd * 1 / itmin);
        y = y + (yd * 1 / itmin);
        z = z - (zd * 1 / itmin);

        //If missile is not burnt out then update mass 
        if (sec <= MOTORBURNTIME)
        {//Amount of mass lost per second subtracted from mass
            MASS = MASS - ((((float)MAXWGT - (float)MINWGT) / (float)MOTORBURNTIME) * 1 / itmin);
        }

        //Distance between target and missile along each axis for current iteration
        newx = (tarx - x);
        newy = (tary - y);
        newz = (tarz - z);

        //Range between missile and target for current iteration
        newr = CMPDIS(newx, newy, newz);

        //X component for line of sight
        xcom = CMPDIS(newx, 0.0f, 0.0f);

        //YZ component for line of sight
        yzcom = CMPDIS(0.0f, newy, newz);

        //Line of sight angle now is
        lnang = (float)Mathf.Atan(yzcom / xcom);

        //Closing rate (how quickly its closing to aircraft) = current distance between missile and target - previous distance of missile and target
        resx = (currx - newx) * itmin;
        resy = (curry - newy) * itmin;
        resz = (currz - newz) * itmin;
        //Relative closing rate
        resr = (currr - newr) * itmin;

        //Closing velocity
        clsvel = -resr;

        //Now employ the guidance algorithm of missile
        LNOFST(newx, newy, newz, resx, resy, resz, ref linex, ref liney, ref linez);
        //linex,liney,linez now has desired velocities that will lead to an intercept

        //Similarly compute desired pitch and yaw rates
        pitcho = CMPOP(yaw, linex, liney);
        yawo = CMPOY(pitch, yaw, linex, liney, linez);

        //Computation of pitch and yaw signals
        pthsig = (float)(G * CMPSIG((float)NAVC, clsvel, pitcho, (float)PTHMAX, (float)MASS, velcty, (float)MINWGT, (float)OPVEL));
        yawsig = (float)(G * CMPSIG((float)NAVC, clsvel, yawo, (float)YAWMAX, (float)MASS, velcty, (float)MINWGT, (float)OPVEL));

        if ((z <= 0.0) || (velcty <= 0.0))
        {
            miss = 1;
            return;
        }

        //Missiles estimated time of closest approach is calculated, along with esitmated range at time of closest approach
        imptme = -((newx * resx) + (newy * resy) + (newz * resz));
        imptme = imptme / (float)((Mathf.Pow(resx, 2) + Mathf.Pow(resy, 2) + Mathf.Pow(resz, 2)));
        range = (float)Mathf.Pow((resx * imptme + newx), 2);
        range = range + (float)Mathf.Pow((resy * imptme + newy), 2);
        range = range + (float)Mathf.Pow((resz * imptme + newz), 2);
        range = (float)Mathf.Sqrt(range);


        //Missile detonation logic
        if (range < DETRNG)
        {//If calculated expected range at time of impact is less than detonation range, then it will hit
            if (Mathf.Abs(imptme) <= intrvl)
            {
                hit = 1;
                return;
            }
        }
       
        if (Mathf.Abs(lnang) > seeker)
        {//if line of sight angle was exceeded, 
            if (newr < DETRNG)
            {//then check if new range is still less than detonation range
                hit = 1;
            }
            else
            {
                return;
            }

            return;
        }
        //non negative closure was achieved between missile and target (missile lost thrust)
        if ((clsvel > 0) && (sec > MOTORBURNTIME))
        {
            if (newr < DETRNG)
            {
                hit = 1;
            }
            else
            {
                miss = 1;
            }
            return;
        }

        return;
    }





    /****************************************************************
    * Name - Function CMPDRG
    * Purpose - Computes drag value for the missile simulation
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * VELCTY REAL meters/second
    * PITCHR REAL radians/second
    * YAWR REAL radians/second
    ****************************************************************/
    private float CMPDRG(float velcty, float pitchr, float yawr)
    {
        float k1 = 0.009412f;
        float k2 = 93850.0f / (float)Mathf.Pow((float)G, 2);
        float drag = (float)(k1 * Mathf.Pow(velcty, 2) + (k2 * Mathf.Pow(pitchr, 2) + Mathf.Pow(yawr, 2)) / Mathf.Pow(velcty, 2));

        return drag;
    }


    /****************************************************************
    * Name - Function CMPSIG
    * Purpose - Computes steering signals for missile
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * NAVC I NTEGER Navigational constant. Usually 4
    * CLSVEL REAL Meters/second
    * LINEO REAL radians (line of sight)
    * MAXTRN REAL G’s (max number of G’s capable of pulling)
    * VELCTY REAL meters/second
     MASS REAL kilograms
    ****************************************************************/

    private float CMPSIG(float navc, float clsvel, float lineo, float maxtrn, float mass, float velcty, float mnmass, float opvel)
    {
        float signal = navc * clsvel * lineo;
        float vlcty2 = velcty * velcty;
        float opvel2 = opvel * opvel;
        float velsig = (vlcty2 / opvel2);
        if (velsig > 1.0)
        {
            velsig = 1.0f;
        }

        float maxsig = (mnmass / mass) * velsig * maxtrn;
        if (Mathf.Abs(signal) > maxsig)
        {
            signal = maxsig * Mathf.Abs(signal) / signal;
        }

        if (Mathf.Abs(signal) > maxtrn)
        {
            signal = maxtrn * Mathf.Abs(signal) / signal;
        }

        return signal;
    }

    /****************************************************************
    * Name - Function CMPOP
    * Purpose - Computes an element used in computing
    * pitch signals from the seeker.
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * YAW REAL radians
    * LINEX REAL meters
    * LINEY REAL meters
    ****************************************************************/
    private float CMPOP(float yaw, float linex, float liney)
    {
        return (float)((-1) * Mathf.Sin(yaw) * linex + Mathf.Cos(yaw) * liney);
    }

    /****************************************************************
    * Name - Function CMPOY
    * Purpose - Computes an element used in computing
    * yaw signals from the seeker.
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * PITCH REAL radians
    * YAW REAL radians
    * LINEX REAL meters
    * LINEY REAL meters
    * LINEZ REAL meters
    ****************************************************************/
    private float CMPOY(float pitch, float yaw, float linex, float liney, float linez)
    {
        double result = Mathf.Sin(pitch) * (Mathf.Cos(yaw) * linex + Mathf.Sin(yaw) * liney);
        result = result + Mathf.Cos(pitch) * linez;
        return (float)result;
    }


    /*  C****************************************************************
    * Name - Subroutine LNOFST
    * Purpose - Computes line of sight angles
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * RX REAL meters
    * RY REAL meters
    * RZ REAL meters
    * RESX REAL meters/second
    * RESY REAL meters/second
    * RESZ REAL meters/second
    * LINEX REAL meters
    * LINEY REAL meters
    * LINEZ REAL meters
    ****************************************************************/
    private void LNOFST(float rx, float ry, float rz, float resx, float resy, float resz, ref float linex, ref float liney, ref float linez)
    {
        float R = CMPDIS(rx, ry, rz);
        float r2 = R * R;
        linex = (ry * resz - rz * resy) / (r2);
        liney = (rz * resx - rx * resz) / (r2);
        linez = (rx * resy - ry * resx) / (r2);

    }

    /****************************************************************
    * Name - Function CMPDIS
    * Purpose - Computes distance
    * Variables -
    * NAME TYPE UNITS
    *----------------------------------------------------------
    * X REAL earth-based x
    * Y REAL earth-based y
    * Z REAL earth-based z
    ****************************************************************/
    private float CMPDIS(float x, float y, float z)
    {
        return (float)Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    }

    /****************************************************************
    * Name - Subroutine GETTAR
    * Purpose - Provides a moving target.
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------------
    * TARX REAL earth-based X
    * TARY REAL earth-based Y
    * TARZ REAL earth-based Z
    * ITMIN INTEGER (# of iterations per second)
    ****************************************************************/
    private void Gettar(ref float tarx, ref float tary, ref float tarz, GameObject targetObject)
    {

        //  float mach = 234.375f;
        tarx = MathCalculations.ConvertUnityToMeters(targetObject.transform.localPosition.x,false);
        tary = MathCalculations.ConvertUnityToMeters(targetObject.transform.localPosition.y,true);
        tarz = MathCalculations.ConvertUnityToMeters(targetObject.transform.localPosition.z,true);
    }

    /*****************************************************************
    * Name - Function CMPTHT
    * Purpose - Computes thrust for missile simulation
    * Variables -
    * NAME TYPE UNITS
    *-------------------------------------------------------------------
    * T INTEGER seconds (# of seconds missile has flown)
    ****************************************************************/
    private float CMPTHT(int T, float maxtrt, float mxtme, float mintrt)
    {
        if (T <= mxtme)
        {
            return maxtrt;
        }
        else
        {
            return mintrt;
        }

    }


    /****************************************************************
    * Name - Function Accel
    * Purpose - Computes acceleration
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * THRUST REAL newtons
    * DRAG REAL newtons
    * PITCH REAL radians
    * MASS REAL kilograms
    ****************************************************************/
    private float ACCEL(float thrust, float drag, float pitch, float mass)
    {
        double accel = ((thrust - drag) / mass) - (G * (Mathf.Sin(pitch)));
        if (accel > MAXACCEL)
            return (float)MAXACCEL;
        return (float)accel;
    }

    /****************************************************************
    * Name - Function PTHACC
    * Purpose - Computes pitch acceleration
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * SIGNAL REAL radians/sec**2
    * PITCHR REAL radians/sec
    ****************************************************************/
    private float PTHACC(float signal, float pitchr)
    {
        float mssct = 0.25f; //Missile time constant
        return (signal - pitchr) / mssct;
    }

    /****************************************************************
    * Name - Function YAWACC
    * Purpose - Computes Yaw acceleration
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * SIGNAL REAL radians/sec**2
    * YAWR REAL radians/sec
    ****************************************************************/
    private float YAWACC(float signal, float yawr)
    {
        float mssct = 0.25f; //Missile time constant    
        return (signal - yawr) / mssct;
    }

    /****************************************************************
    * Name - Function CNGPTH
    * Purpose - Computes changes in the pitch angle
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * PITCHR REAL radians/second
    * VELCTY REAL meters/second
    * PITCH REAL radians
    ****************************************************************/
    private float CNGPTH(float pitchr, float velcty, float pitch)
    {
        return (float)((pitchr - Mathf.Cos(pitch)) / velcty);
    }

    /****************************************************************
    * Name - Function CNGYAW
    * Purpose - Computes yaw changes
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * YAWR REAL radians/second
    * VELCTY REAL meters/second
    * PITCH REAL radians
    ****************************************************************/
    private float CNGYAW(float yawr, float velcty, float pitch)
    {
        return (float)(yawr / (velcty * (Mathf.Cos(pitch))));
    }

    /****************************************************************
    * Name - Function CNGX
    * Purpose - Computes changes in x direction
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * VELCTY REAL meters/second
    * PITCH REAL radians
    * YAW REAL radians
    ****************************************************************/
    private float CNGX(float velcty, float pitch, float yaw)
    {
        return (float)(velcty * Mathf.Cos(pitch) * Mathf.Cos(yaw));
    }


    /****************************************************************
    * Name - Function CNGY
    * Purpose - Computes changes in y direction
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * VELCTY REAL meters/second
    * PITCH REAL radians
    * YAW REAL radians
    ****************************************************************/
    private float CNGY(float velcty, float pitch, float yaw)
    {
        return (float)(velcty * Mathf.Cos(pitch) * Mathf.Sin(yaw));
    }

    /****************************************************************
    * Name - Function CNGZ
    * Purpose - Computes changes in z direction
    * Variables -
    * NAME TYPE UNITS
    *------------------------------------------------------
    * VELCTY REAL meters/second
    * PITCH REAL radians
    * YAW REAL radians
    ****************************************************************/
    private float CNGZ(float velcty, float pitch)
    {
        return (float)(velcty * Mathf.Sin(pitch));
    }
}
